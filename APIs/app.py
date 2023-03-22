import cv2
import numpy as np
from langdetect import detect
from googletrans import Translator
from flask import Flask, jsonify, request

app = Flask(__name__)

# API endpoint to handle face_detection requests
@app.route("/api/face_detection", methods=["POST"])
def face_detection():
    try:
        # Load the cascade classifier
        face_cascade = cv2.CascadeClassifier(
            cv2.data.haarcascades + 'haarcascade_frontalface_default.xml')

        # Get the image data from the request
        img_data = request.data

        # Convert the image data to a NumPy array
        arr = np.frombuffer(img_data, np.uint8)

        # Decode the image from the NumPy array
        img = cv2.imdecode(arr, cv2.IMREAD_COLOR)

        # Convert into grayscale
        gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)

        # Detect faces
        faces = face_cascade.detectMultiScale(
            gray, scaleFactor=1.1, minNeighbors=5)

        # Return 1 if face is present, 0 otherwise
        if len(faces) == 0:
            return jsonify({"Result": "No face found in the image, you may use it.", "Code": 0})
        else:
            return jsonify({"Result": "Face found in the image. Kindly Choose a non facial image", "Code": 1})
    except Exception:
        return jsonify({"Error": "No image was provided or Image has broken.", "Code": 400}), 400

# API endpoint to handle translation requests
@app.route('/api/translate', methods=['POST'])
def translate_input():
    # parse JSON request body
    try:
        request_body = request.get_json()
        if not request_body or 'text' not in request_body or 'language' not in request_body:
            raise ValueError('Invalid request body.')
    except ValueError as e:
        return jsonify({'error': str(e)}), 400

    # get user input
    try:
        user_input = request_body['text']
        if user_input is None or not user_input.strip():
            return jsonify({'error': 'Text field missing from request body.'}), 400
    except KeyError:
        return jsonify({'error': 'Text field missing from request body.'}), 400

    # detect input language
    try:
        input_language = detect(user_input)
    except Exception:
        return jsonify({'error': 'Could not detect input language.'}), 400

    # perform translation
    try:
        target_language = request_body['language']
        translator = Translator()
        translation = translator.translate(
            user_input, src=input_language, dest=target_language)
    except KeyError:
        return jsonify({'error': 'Language field missing from request body.'}), 400
    except ValueError:
        return jsonify({'error': 'Invalid language.'}), 400
    except Exception:
        return jsonify({'error': 'Translation failed.'}), 500

    # return translation result as JSON response
    return jsonify({'translation': translation.text}), 200



if __name__ == "__main__":
    app.run()
