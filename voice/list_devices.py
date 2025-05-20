import sounddevice as sd

print("\nAvailable audio devices:\n")
print(sd.query_devices())

print("\nDefault input device:", sd.default.device)
