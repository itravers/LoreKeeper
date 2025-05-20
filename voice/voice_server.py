from flask import Flask, request, jsonify
from TTS.api import TTS
import os
import uuid

app = Flask(__name__)
tts = TTS(model_name="tts_models/en/vctk/vits", progress_bar=False, gpu=False)

OUTPUT_DIR = "output_audio"
os.makedirs(OUTPUT_DIR, exist_ok=True)

@app.route("/narrate", methods=["POST"])
def narrate():
    data = request.get_json()
    if not data or "text" not in data:
        return jsonify({"status": "error", "message": "Missing 'text' field"}), 400

    text = data["text"].strip()
    if not text:
        return jsonify({"status": "error", "message": "Empty text"}), 400

    speaker = data.get("speaker")
    custom_path = data.get("path")
    if custom_path:
        output_path = os.path.join(OUTPUT_DIR, os.path.basename(custom_path))
    else:
        filename = f"{uuid.uuid4().hex}.wav"
        output_path = os.path.join(OUTPUT_DIR, filename)

    try:
        tts.tts_to_file(text=text, speaker=speaker, file_path=output_path)
        return jsonify({"status": "ok", "path": os.path.abspath(output_path)})
    except Exception as e:
        return jsonify({"status": "error", "message": str(e)}), 500

@app.route("/speakers", methods=["GET"])
def list_speakers():
    return jsonify({"speakers": tts.speakers})

if __name__ == "__main__":
    app.run(port=5002)
