# BG3Mod Highlight Everything Revised
The code will read through all the game's pak files with the help of [LSLib](https://github.com/Norbyte/lslib)., skipping the texture paks, from the oldest to newest order which works well for patches.

It will pick out .LSF files related items, all located in RootTemplates and Items folders while respecting the overrides done by the patches.

You can broaden the search by removing the search criteria but that just unnecessarily increasing the runtime.

It will further narrow down the files based on required modifications which are super parent, existing tooltip and to-excluded map key.

It then further narrow down by extracting the objects of interests to be compiled into representative mod files.

Ultimately, it will package and output an zip mod.


Highlight Everything Revised work on the principle of whitelist by default aka every tooltip is enabled via super parents and removing existing children's tooltip setting to inherent super parents.

Tooltip that need to be disabled must be expliclity included in "excludeMapKeyList" inside the code.
