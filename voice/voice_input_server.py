from flask import Flask, jsonify
from faster_whisper import WhisperModel
import sounddevice as sd
import numpy as np
import scipy.io.wavfile as wav
import tempfile

app = Flask(__name__)
model = WhisperModel("tiny", device="cpu", compute_type="float32")

@app.route("/listen", methods=["POST"])
def listen_and_transcribe():
    SAMPLE_RATE = 16000
    DURATION = 5
    sd.default.device = (2, None)  # Set your mic device index

    print("[Voice Server] Recording...")
    recording = sd.rec(int(DURATION * SAMPLE_RATE), samplerate=SAMPLE_RATE, channels=2, dtype='int16')
    sd.wait()
    recording = np.mean(recording, axis=1).astype('int16')

    with tempfile.NamedTemporaryFile(suffix=".wav", delete=False) as tmpfile:
        wav.write(tmpfile.name, SAMPLE_RATE, recording)
        segments, _ = model.transcribe(tmpfile.name)
        transcript = " ".join([seg.text for seg in segments]).strip()

    print(f"[Voice Server] Transcript: {transcript}")
    return jsonify({ "transcript": transcript })

if __name__ == "__main__":
    app.run(port=5003)
