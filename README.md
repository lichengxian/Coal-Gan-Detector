# Project Introduction
This is a C# project on Win10. This project adpots deep learning method (object detection) to distinguish coal and gan, and uses machine gripper to sort the detected coal. The project can be divided into three parts: Detect Part, Camera Part and Data Communication Part.
# Detect Part
We use [YOLOv4-tiny](https://github.com/AlexeyAB/darknet) model as the detector to detect and classify coal and gan. The trained model runs on CPU, and compiles without OpenCV. 

The Detector accepts the image catched from Camera, then locate and classify coal or gan on the image. The detect results will send to machine gripper and visualize in window.

# Preject File
- **Form1.Designer.cs:** Window design code.
- **Form1.cs:** Including Camera, Detector, Data communication, MySQL and other codes.
- **CommClass.cs:** About serial port.
- **Program.cs:** C# main program entry point.
- **YoloWrapper.cs:** YOLO detect model wrapper code, copy from `darknet/build/darknet/YoloWrapper.cs`.
