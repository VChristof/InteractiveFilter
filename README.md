# InteractiveFilter
Add a new interactive filter for KSP2 OAB to filter parts dynamically.

![20230323143135_1](https://user-images.githubusercontent.com/127409491/227220648-16482924-db13-4a8d-90ee-1b4ab62bce73.jpg)

This mod adds two new buttons:
- Top filter (Can be switched by key "T" also)
- Bottom filter (Can be switched by key "B" also)

Activating them will filter the parts shown.

Top will only show the parts fitting on the top free attachment nodes.

Bottom will only show the parts fitting on the bottom free attachment nodes.

Orientation is calculated from the assembly's anchor part.

Only the launch vehicle is considered in the filltering.

If both filter is active attachment node orientation is note checked only if its free. (every node's size is considered)

If there are no free nodes no filtering will be applied.
