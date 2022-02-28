# Eye-Gaze Head-Gaze Combined Interaction

## Requirements
- Unity 2020.3.24f1
- [Magic Leap SDK with The Lab](https://developer.magicleap.com/en-us/learn/guides/lab)
- Optional: [Jupyter Notebook Analysis Project](https://github.com/kevinskyba/EGHG-Analyzer)

## How To Run
Currently, this app does not support running natively on Magic Leap because of some IL2CPP bug. Running the app follows the regular procedure for running a Unity App with [Zero Iteration](https://developer.magicleap.com/en-us/learn/guides/lab-zi).

In each scene, there is an `EGHGInputManager` GameObject, which contains an `InputManager` Behaviour, which has a Mode. `EGHG` stands for the experimental approach combining both Eye- and Head-Gaze. `EyeGaze` stands for the baseline approach.

The Jupyter Notebook Analysis Project is only available for the `EGHG` approach and requires the `Pandas Connector` to be enabled.

## Demo Video

[Youtube](https://www.youtube.com/watch?v=3hvjSWVDGhA)

## Explanation

[PDF](Doc/main.pdf)