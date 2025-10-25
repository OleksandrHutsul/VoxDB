window.vox = (function () {
    let recognition = null;
    let mediaRecorder = null;
    let stream = null;
    let audioChunks = [];
    let callbackRef = null;
    let mode = 'auto'; 
    let sessionToken = 0; 

    function isSpeechAvailable() {
        return !!(window.SpeechRecognition || window.webkitSpeechRecognition);
    }

    function setMode(newMode) { mode = newMode || 'auto'; }

    async function startListening(dotnetRef) {
        const token = ++sessionToken;

        callbackRef = dotnetRef;
        audioChunks = [];

        stream = await navigator.mediaDevices.getUserMedia({ audio: true });

        mediaRecorder = new MediaRecorder(stream);
        mediaRecorder.ondataavailable = e => {
            if (token !== sessionToken) return;
            audioChunks.push(e.data);
        };
        mediaRecorder.start();

        const primaryLang = mode === 'en' ? 'en-US' : 'uk-UA';
        const fallbackLang = mode === 'en' ? 'uk-UA' : 'en-US';

        const got = await recognizeOnce(token, primaryLang);
        if (!got && mode === 'auto') {
            await recognizeOnce(token, fallbackLang);
        }
    }

    function ensureStopRecognition() {
        try { if (recognition) recognition.stop(); } catch { }
        recognition = null;
    }

    function ensureStopRecorder() {
        return new Promise((resolve) => {
            if (!mediaRecorder) return resolve();
            try {
                if (mediaRecorder.state !== 'inactive') {
                    mediaRecorder.onstop = () => { resolve(); };
                    mediaRecorder.stop();
                } else {
                    resolve();
                }
            } catch {
                resolve();
            } finally {
                mediaRecorder = null;
            }
        });
    }

    function ensureStopStream() {
        try {
            if (stream) {
                stream.getTracks().forEach(t => { try { t.stop(); } catch { } });
            }
        } catch { }
        stream = null;
    }

    async function cleanupAll() {
        ensureStopRecognition();
        await ensureStopRecorder();
        ensureStopStream();
    }

    async function recognizeOnce(token, lang) {
        const SR = window.SpeechRecognition || window.webkitSpeechRecognition;
        recognition = new SR();
        recognition.lang = lang;
        recognition.interimResults = false;
        recognition.maxAlternatives = 1;

        return new Promise((resolve) => {
            let delivered = false;

            recognition.onresult = async (event) => {
                if (token !== sessionToken) return resolve(false);

                delivered = true;

                ensureStopRecognition();

                await ensureStopRecorder();
                const audioUrl = await buildAudioUrl();

                const text = (event.results?.[0]?.[0]?.transcript || '').trim();

                if (callbackRef) {
                    try { await callbackRef.invokeMethodAsync('OnTranscript', text, audioUrl); } catch { }
                }

                ensureStopStream();

                resolve(!!text);
            };

            recognition.onerror = async () => {
                if (token !== sessionToken) return resolve(false);
                await cleanupAll();
                resolve(false);
            };

            recognition.onend = async () => {
                if (token !== sessionToken) return resolve(false);
                if (!delivered) {
                    await cleanupAll();
                    if (callbackRef) {
                        try { await callbackRef.invokeMethodAsync('OnTranscript', "", null); } catch { }
                    }
                    resolve(false);
                }
            };

            try { recognition.start(); } catch {
                resolve(false);
            }
        });
    }

    async function buildAudioUrl() {
        if (!audioChunks || audioChunks.length === 0) return null;
        try {
            const blob = new Blob(audioChunks, { type: 'audio/webm' });
            return URL.createObjectURL(blob);
        } catch {
            return null;
        } finally {
            audioChunks = [];
        }
    }

    async function stopListening() {
        sessionToken++; 
        await cleanupAll();
    }

    return { isSpeechAvailable, startListening, stopListening, setMode };
})();
