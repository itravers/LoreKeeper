from faster_whisper import WhisperModel
import sounddevice as sd
import numpy as np
import tempfile
import scipy.io.wavfile as wav
import os

SAMPLE_RATE = 16000
DURATION = 5

sd.default.device = (2, None)  # replace with your confirmed mic index

print("[Voice Input] Listening...")
recording = sd.rec(int(DURATION * SAMPLE_RATE), samplerate=SAMPLE_RATE, channels=2, dtype='int16')
sd.wait()

recording = np.mean(recording, axis=1).astype('int16')

with tempfile.NamedTemporaryFile(suffix=".wav", delete=False) as tmpfile:
    wav.write(tmpfile.name, SAMPLE_RATE, recording)
    temp_path = tmpfile.name

print(f"[DEBUG] Audio saved to: {temp_path} ({os.path.getsize(temp_path)} bytes)")

print("[Voice Input] Loading model...")
model = WhisperModel("tiny", device="cpu", compute_type="float32")



print("[Voice Input] Starting transcription...")
segments, info = model.transcribe(temp_path)
print("[Voice Input] Finished transcription")

transcript = " ".join([seg.text for seg in segments]).strip()
if transcript:
    print("Transcript: " + transcript)
else:
    print("[ERROR] No transcription result.")
