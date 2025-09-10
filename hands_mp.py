import cv2
import mediapipe as mp
import socket
import json

# UDP setup
UDP_IP = "127.0.0.1"
UDP_PORT = 5005
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

# Initialize MediaPipe Hands
mp_hands = mp.solutions.hands
mp_drawing = mp.solutions.drawing_utils

# parameters
width, height = 1230, 720

# Open webcam
cap = cv2.VideoCapture(0)
cap.set(3, width)  # Width
cap.set(4, height)  # Height


with mp_hands.Hands(
    static_image_mode=False,
    max_num_hands=2,
    min_detection_confidence=0.5,
    min_tracking_confidence=0.5
) as hands:
    while cap.isOpened():
        ret, frame = cap.read()
        if not ret:
            break

        # Convert the BGR image to RGB
        image = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
        image.flags.writeable = False

        # Process the image and find hands
        results = hands.process(image)

        # Draw hand landmarks and send data
        image.flags.writeable = True
        image = cv2.cvtColor(image, cv2.COLOR_RGB2BGR)
        landmarks_data = []
        if results.multi_hand_landmarks:
            # for hand_landmarks in results.multi_hand_landmarks:
            # Only process the first detected hand
            hand_landmarks = results.multi_hand_landmarks[0]
            mp_drawing.draw_landmarks(
                image, hand_landmarks, mp_hands.HAND_CONNECTIONS)
            # Extract landmark coordinates
            hand = []
            for lm in hand_landmarks.landmark:
                landmarks_data.extend([lm.x * width, (1-lm.y)*height , lm.z * width])
            # landmarks_data.extend(hand)
            print(landmarks_data)
            # Send data as JSON string
            sock.sendto(json.dumps(landmarks_data).encode(), (UDP_IP, UDP_PORT))

        cv2.imshow('MediaPipe Hands', image)
        if cv2.waitKey(1) & 0xFF == 27:
            break

cap.release()
cv2.destroyAllWindows()
sock.close()