# RetroStation - A GUI Front-End for Emulators.

[Download Latest Version](https://github.com/KieranMcCool/RetroStation/blob/master/Versions/RetroStationLatest.zip?raw=true)

**Note: Very early release, getting this working might be a challenge just now. 
Optimizations and fixes coming soon.

## What is RetroStation?

RetroStation is an open source program designed to act as a graphical front-end for
emulators allowing basic statistics tracking and easy ROM management.

## How does it work?

You set the program up with platforms, each platform has an associated ROM type and
emulator which it will use to run it's ROMs. You can set up as many platforms as you
like and the ROM files are passed in to the emulator through the use of command line
arguments so you'll need to make sure your emulator supports that feature (most do.)

Custom command line arguments can be created to allow more control over how the emulator
works but make sure you include "{0}" in it somewhere as this is what is replaced with
the ROM files path.

## There are hundreds of GUI front-ends for emulators, why RetroStation?

Use what you like, I made this because I couldn't find one that suited my needs. They
were all either too much hassle to set up, lacked decent controller support or were
useless if you were using anything other than a controller. I believe I've hit a decent
middle ground here. It's the jack of all trades (and the master of none) I haven't
invested too much effort into making a fancy GUI that has big icons and fancy text for
using a controller, nor have I created lots of nested menus with configurable options to
maximize customization and features when using the keyboard and mouse.

It's just a simple, easy to use interface which can use both a keyboard and mouse or a
controller as the situation requires.

It's also very easy to set up allowing batch imports of ROM files, where you can dump
all of your ROMs in one folder and import them easily often without having to provide
any more input. EZPZ.

## Which emulators does it support?

It supports anything so long as it takes command line arguments for a file path of the
ROM. I can confirm that it works for Mupen64plus, Dolphin, Mednafen, VBA, PSX and
Project64. I make no guarantees that it will always support these (I don't control
whether the developers decide to drop support for Command line arguments or not) and will not
assist in setting up any other emulator. The default command line template will work
with most but some may require some tweaking, in any case: Google is your best friend. 

## Controller Support?

**Yes.**

It's a bit clunky and I'm still torn on whether to use OpenTK.Input or SharpDx. I want
to port this to Linux eventually (running under Mono) but I can't get OpenTK to work on
Mono. I'm currently using SharpDX as it's easier to work with than OpenTK and since
neither will work on Mono under Linux it makes sense to use the easiest solution since
'm gaining nothing from using OpenTK.

## Currently no remapping for controls (coming soon)

As a result, here are the default controls for using a controller:
* DPadUp - Move upward through the games list.
* DPadDown - Move down through the games list.
* Left Shoulder - Cycle through the Platform filter to the left.
* Right Shoulder - Cycle through the Platform filter to the right.
* A Button - Launch the selected game.
* Start, Back/Select, LeftShoulder & Right Shoulder - Exits the running emulation (if one is 
running) and returns you to the program.


## When will you support Linux?

As soon as I can get OpenTK working with Mono or find some other library which suits my
needs. I don't want to give Linux users a half working product. Go hard or go home, you
get it all or you don't.

So until I can support it fully I'm afraid you're out of luck. The basic program does
work using Wine though which may be some (small) consolation.

## Where can I get ROMs?

How you obtain them is entirely your responsibility, I will not provide links or
instructions on where to get them. Emulation is a very gray area legally and I don't
want to be involved in anything sketchy. 

**This program includes NO ROM files and NO
emulation software, please don't ask me about obtaining such things as I will simply
ignore you.**

## Why the name RetroStation?

I wanted to call it EmulationStation because it rhymes and stuff but turns out there's a
similar project to this with that name. I promise that I haven't copied their ideas at
all and that this program was actually feature complete and ready for distribution before
I found out their project existed. The work they've done is great and I urge you to
check them out if you find this program to be inadequate for your needs. 

If the developers of EmulationStation stumble on this program and wish to contact me with any
concerns then please don't hesitate. No harm is intended.
