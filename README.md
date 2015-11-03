# RetroStation - A GUI Front-End for Emulators.

[Download Latest Version](https://github.com/KieranMcCool/RetroStation/blob/master/Versions/RetroStationLatest.zip?raw=true)

## What is RetroStation?

RetroStation is an open source program designed to act as a graphical front-end for
emulators. It allows basic statistics tracking and simple ROM management.

## How does it work?

You create "Platforms" inside the program, each platform represents a Game Console or
similar device, the platform you create will have an associated emulator which will be
used to launch any ROM Files registered to that platform.

Given that some Emulators work better than others on certain games, it is entirely
possible and sometimes preferable to be create two platforms for once console which
allows you to pick and choose which emulator to use on a game by game basis.

The emulator is launched by passing a ROM file to it using the command line arguments,
as such, this will only work on emulators which support this feature. (Most do) Keep
this in mind when selecting your emulators.

Custom command line arguments can be created to allow more control over how the emulator
behaves but make sure you include "{0}" in it somewhere as this is what is replaced with
the ROM files path.

## There are hundreds of GUI front-ends for emulators, why RetroStation?

I made this because I couldn't find one that suited my needs. They
were all either too much hassle to set up, lacked decent controller support or were
useless if you were using a keyboard and mouse. I sometimes prefer a controller over the
keyboard and mouse other times the opposite is true. I believe I've hit a decent middle 
ground with this solution. It's designed to be manageable using a controller or a keyboard
and mouse. *Some of the more advanced features are only accessible using the
mouse though.* 

It's just a simple, easy to use interface that gets the job done.

It's also very easy to set up allowing batch imports of ROM files, where you can dump
all of your ROMs in one folder and import them easily, if you have all your platforms
setup then you may not even need to provide any more input.

## Which emulators does it support?

It supports any emulator which allows ommand line arguments to the file path of the
ROM. I can personally confirm that it works for Mupen64plus, Dolphin, Mednafen, VBA, PSX and
Project64. I make no guarantees that it will always support these (I don't control
whether the developers decide to drop support for Command line arguments or not) and will not
assist in setting up any other emulator. The default command line template will work
with most but some may require some tweaking, in any case: Google is your best friend. 

## Controller Support?

**Yes.**

I'm currently using a library called [SharpDX](http://sharpdx.org/) for controller input, eventually I hope
to move to something which will work cross-platform so I can port this program to Linux
and OSX under the Mono framework. As of now, the program will compile under Mono but
since SharpDX is a wrapper for DirectX's xInput the module will not work on anything
other than Windows. Hopefully this can change in the future when I find a library which
will work all platforms.

## Currently no remapping for controls (planned)

As such, here are the default controls for using a controller:
* DPadUp - Move upward through the games list.
* DPadDown - Move down through the games list.
* Left Shoulder - Cycle through the Platform filter to the left.
* Right Shoulder - Cycle through the Platform filter to the right.
* A Button - Launch the selected game.
* Start, Back/Select, LeftShoulder & Right Shoulder - Exits the running emulation (if one is 
running) and returns you to the program.


## When will you support Linux/OSX?

As soon as I can get the controller input working with Mono.
I don't want to give Linux users a half working product.

So until I can support it fully I'm afraid you're out of luck. The basic program does
work using Wine though which may be some (small) consolation.

If you find a library which can acheive this, drop me a message and I'll look into it.

Update: MonoGame may be what I'm looking for regarding this, I'll be experimenting with
it soon.

## Where can I get ROMs?

How you obtain them is entirely your responsibility, I will not provide links or
instructions on where to get them. Emulation is a very gray area legally and I don't
want to be involved in anything sketchy. 

**This program includes NO ROM files and NO
emulation software, please don't ask me about obtaining such things as I will simply
ignore you.**

## Why the name RetroStation?

I wanted to call it EmulationStation because it rhymes but it turns out there's a
similar project with that name. I promise that I haven't copied their ideas at
all and that this program was actually in development before I found out their project 
existed. The work they've done is great and I urge you to check them out if you find 
this program to be inadequate for your needs. 

If the developers of EmulationStation stumble on this program and wish to contact me with any
concerns then please don't hesitate. 

## Libraries Used

* [SharpDX](http://sharpdx.org/) - MIT License
	- Used for controller input. 
