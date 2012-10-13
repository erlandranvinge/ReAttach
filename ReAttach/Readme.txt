ReAttach

Overview

ReAttach gives you an easy way to ReAttaching to prior debug target. This is especially useful when working with IIS-processes. ReAttach stores your attach history for you and provides both toolbar and menu options for easy access. 

The hotkey sequence CTRL+R, CTRL+A is used to ReAttach to your latest target (top of the history list). If your process is not currently running, ReAttach will ask you to start it and attach to it as soon as the process becomes available.

How does it work?

ReAttach stores process path, username and PID in the registry (in a subkey called ReAttach located under the currently running visual studio user's key). Once a ReAttach is made the following steps occurs:

All candidates (running processes) with matching path and username are filtered out.
If there's a candidate with a matching PID that process is immediately selected and attached to.
The process matching username and path with the highest PID is selected and attached to.
If no match is found, the user is presented with a modal dialog telling him/her to start the process or cancel.