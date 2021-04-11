# InputHistory
A visual input log for use as an [OBS](https://github.com/obsproject/obs-studio) source. The default settings are designed to work with a luma key filter to remove the black background of the window. Throw the source through a greyscale filter before using the luma key to remove the colored fringing of ClearType's subpixel font rendering.


## Table of Contents  
1. [Configuration](#Configuration)  
2. [Limitations](#Limitations)
3. [Settings](#Settings)  
  i. [BgColor](#BgColor)  
  ii. [FontName](#FontName)  
	iii. [FontSize](#FontSize)  
	iv. [Width](#Width)  
	v. [Height](#Height)  
	vi. [CoalesceMashing](#CoalesceMashing)  
	vii. [ShowFatfingers](#ShowFatfingers)  
	viii. [SeparateOutDiagonalDPadInputs](#SeparateOutDiagonalDPadInputs)  
	ix. [MaxEntries](#MaxEntries)  
	x. [BindingRepresentations](#BindingRepresentations)  

If all you're concerned about configuring is what image or text InputHistory displays when you press a button, go to [BindingRepresentations](#BindingRepresentations). Maybe bring a snack.

## Configuration
After running and closing InputHistory for the first time, the program will save a settings file named `user.config` to `%LOCALAPPDATA%\InputHistory\InputHistory_Url_{...}\{VersionString}`, where `{...}` is some alphanumeric string and `{VersionString}` is the current version of the progam. This is an XML file and can be opened in any text editor, although an editor with extra support for XML files (such as [VS Code](https://github.com/Microsoft/vscode)) will be helpful.

The relevant settings inside `user.config` are stored between the lines `<InputHistory.Properties.Settings>` and `</InputHistory.Properties.Settings>`. Each setting is stored in the following format:
```xml
<setting name="GenericName" serializeAs="String">
  <value>GenericValue</value>
</setting>
```
There are a handful of available settings. Their name replaces `GenericName`, and their value replaces `GenericValue`. If the setting you want isn't in `user.config`, you can add it to the file between the `<InputHistory.Properties.Settings>` and `</InputHistory.Properties.Settings>` lines. Make sure it's not in the middle of another settings entry.
This is fine:
```xml
<setting name="Width" serializeAs="String">
  <value>2572</value>
</setting>
<setting name="Height" serializeAs="String">
  <value>104</value>
</setting>
```
This will cause problems:
```xml
<setting name="Width" serializeAs="String">
  <value>2572</value>
<setting name="Height" serializeAs="String">
  <value>104</value>
</setting>
</setting>
```
If something's well and truly broken, deleting a settings entry or even the whole `user.config` file should be fine, at the cost of losing whatever configuration you just deleted.

## Limitations
There are a few things InputHistory cannot do. There are a few things it cannot do *yet*.
### Limitations to be addressed in the future:
* InputHistory currently only looks at the controller assigned to Player 1. This will be fixed once I figure out a good way to pick which controller(s) to display.
* Configuring an Override to require multiple button presses to be held is not, at the moment, possible. This will be dealt with.
* InputHistory currently has no settings window; the settings file must be edited by hand. Instructions for this are below. The settings window will take a lot of design to do properly.
* InputHistory fires no matter what's going on; eventually, there will be an option to only log input while a particular program is focused.
### Limitations that may not be addressed in the future:
* Subpixel font rendering's color fringes cause problems when attempting to mask out the window background, but I am not sure if it is even possible to render antialiased font without it in WPF, unless subpixel rendering is turned off at a system level.
* The need to mask out the background at all instead of having native transparency. OBS, alas, does not capture transparent windows properly, leaving this solution the best available to me at this time.



## Settings
### BgColor
Default:
```xml
<setting name="BgColor" serializeAs="String">
  <value>Black</value>
</setting>
```
This is the background color of the window. The default black, along with the white and light grey elements, is intended to be used with OBS's luma key filter to mask out the background (as OBS cannot capture transparent windows properly).

Put a CSS hex code like `#88FF00` between `<value>` and `</value>` or any one of the color names in the following chart (no spaces): ![WPF-color-name-chart](https://docs.microsoft.com/en-us/dotnet/media/art-color-table.png?view=net-5.0)

### FontColor
Default:
```xml
<setting name="FontColor" serializeAs="String">
	<value>White</value>
</setting>
```
This is the font color of the text inside the window. As mentioned in the entry for [BgColor](#BgColor), the default color (white), is designed for OBS's luma key filter. This setting accepts the same values [BgColor](#BgColor) does.

### FontName
Default:
```xml
<setting name="FontName" serializeAs="String">
	<value>Segoe UI</value>
</setting>
```
This is the font that is used for the text inside the window. The name of any font you have installed on your machine will work, but it must be an exact match (spaces, etc.). Pick something you like :)

### FontSize
Default:
```xml
<setting name="FontSize" serializeAs="String">
	<value>20</value>
</setting>
```
The font size for the text inside the window. Any positive number should work.

### Width
```xml
<setting name="Width" serializeAs="String">
	<value>720</value>
</setting>
```
The width of the window. InputHistory will remember what you resize the window to with the mouse and write it down here. Any positive whole number should work.

### Height
```xml
<setting name="Height" serializeAs="String">
	<value>104</value>
</setting>
```
The height of the window. InputHistory will remember what you resize the window to with the mouse and write it down here. Any positive whole number should work.

### CoalesceMashing
```xml
<setting name="CoalesceMashing" serializeAs="String">
	<value>False</value>
</setting>
```
If this option is enabled (`<value>True</value>` instead of `<value>False</value>`), InputHistory will collect repeated inputs of the same button into one entry in the list. A coalesced entry displays how many times it's been pressed as well as the frequency it's been pressed. 

### ShowFatfingers
```xml
<setting name="ShowFatfingers" serializeAs="String">
	<value>False</value>
</setting>
```
If this option is enabled (`<value>True</value>` instead of `<value>False</value>`), InputHistory will log inputs it has no configured representation for as "Fatfinger".


### SeparateOutDiagonalDPadInputs
Default:
```xml
<setting name="SeparateOutDiagonalDPadInputs" serializeAs="String">
	<value>True</value>
</setting>
```
When enabled, pressing two directions at the same time on the dpad of a controller will produce a diagonal log entry. That is, pressing up and left on the dpad will produce an up-left entry (usually, either an up or left entry followed by an up-left, as it is difficult to press both at the same). When disabled (set to `<value>False</value>`), pressing up-left on the dpad will produce an up entry and a separate left entry (the order depending on exactly how you pressed the dpad, probably the controller hardware to some extent, and almost certainly the phase of the moon, in descending orders of magnitude).

### MaxEntries
Default:
```xml
<setting name="MaxEntries" serializeAs="String">
	<value>127</value>
</setting>
```
This is how many entries InputHistory will keep in RAM. The default for this setting is a fair bit on the large side, but adjusting it up or down shouldn't have much effect unless you crank it down low enough to throw away entries that are still visible. In a future update, InputHistory will automatically discard entries that are no longer visible and this option will go away.

### BindingRepresentations
Default:
```xml
<setting name="BindingRepresentations" serializeAs="String">
	<value/>
</setting>
```
This is the weird one.

In the actual `user.config` file, the value in the settings is going to be something like
```json
				<value>{
	"None": {
    "DefaultRepresentation": "",
    "Overrides": []
  },
  "XInputDPadUp": {
    "DefaultRepresentation": "up.png",
    "Overrides": []
  },
  "XInputDPadDown": {
    "DefaultRepresentation": "down.png",
    "Overrides": []
  },
  "XInputDPadLeft": {
    "DefaultRepresentation": "left.png",
    "Overrides": []
  },
  "XInputDPadRight": {
    "DefaultRepresentation": "right.png",
    "Overrides": []
  },
  "XInputDPadUpLeft": {
    "DefaultRepresentation": "up left.png",
    "Overrides": []
  },
  "XInputDPadUpRight": {
    "DefaultRepresentation": "up right.png",
    "Overrides": []
  },
  "XInputDPadDownLeft": {
    "DefaultRepresentation": "down left.png",
    "Overrides": []
  },
  "XInputDPadDownRight": {
    "DefaultRepresentation": "down right.png",
    "Overrides": []
  }//more entries after this...
				}</value>
```
This particular setting is configured differently from the others. Between `<value>` and `</value>`, it uses a format known as JSON. Each entry in the `BindingRepresentations` list is structured as follows:
```json
	/*Name of input:*/ "RightMouseButton": {//name must be between a pair of ""s.
	//For a complete list of input names, see below
	//Either text to display when this↑ input ("RightMouseButton") is pressed or a path to an image file to use instead:
	//↳----------------------↴-----↴
	"DefaultRepresentation": "Block", //←whether text or filepath, this must also be between ""s
	//If the text between the quotes after "DefaultRepresentation": contains a period
	//(for example, "up.png"), InputHistory will interpret that text as a filepath
	//and will attempt to use whatever's at that filepath as an image.
	//For example, if RightMouseButton was pressed without any of the Overrides listed below
	//being held down, then it will be displayed using the text "Block".
	//However, if, after the : was "block.png", InputHistory would display the image at the path
	//"block.png".

	//You can also supply a list of Overrides.
	//An Override is an alternate representation for an input,
	//shown when any of the inputs listed in "Codes" are currently active
	//(for instance, a trigger or a button is being held down).
	//
	//If you have more than one Override in the Overrides list,
	//the first one that's applicable is used.
	//
	//For example, if just W was being held down when RightMouseButton was both pressed,
	//then the input would be displayed with the text "Dodge".
	//
	//If both LeftShift and W were down when RightMouseButton was pressed,
	//then the result would be the text "Shortcut 3",
	//as that Override comes first in the Overrides list.
	"Overrides": [ 
		{
			"Representation": /*Text or a filepath again*/ "Shortcut 3",
			"Codes": [
				"LeftShift"
			]
		},
		{
			"Representation": "Dodge", //you can have as many Overrides as you want
			"Codes": [
				"W",//You can have more than one code in an Override;
				"A",//if any of them are being held, the Override applies.
				"S",
				"D"
			]
		}
	]
}
```
Each input listed on the right side of the colons, the ones at the top level of the list, must be unique. So this is ok:
```json
<value>{
	"W": {
		"DefaultRepresentation": "Forward",
		"Overrides": []
	},
	"LeftCtrl": {
		"DefaultRepresentation": "Crouch",
		"Overrides": [
			{
				"Representation": "Slide",
				"Codes": [
					"W"
				]
			}
		]
	}
}</value>
```
But this will cause your configuration to be ignored and overwritten with the default mapping when InputHistory is closed:
```json
<value>{
	"W": {
		"DefaultRepresentation": "Forward",
		"Overrides": []
	},
	"W": { //duplicate W entry in the top-level list!!
		"DefaultRepresentation": "Forward",
		"Overrides": []
	}
}</value>
```
Please note the locations and details of all the brackets and punctuation. JSON, like most computer languages, is rather particular about its punctuation. Also, including a `>` or a `<` in the JSON anywhere will probably cause problems. It's still inside an XML file, and XML is very particular about its `<`s and `>`s.

And, finally, here is a list of all the input names:

Special:
```c
None //This is used to denote no (neutral) input. If you don't want this to show up, remove it from the list of BindingRepresentations.
```	

Keyboard:
```c
Cancel, Back, Tab, LineFeed, Clear, Enter, Return, Pause, Capital, CapsLock,
HangulMode, KanaMode, JunjaMode, FinalMode, HanjaMode, KanjiMode,
Escape,
ImeConvert, ImeNonConvert, ImeAccept, ImeModeChange,
Space, PageUp, Prior, Next, PageDown, End, Home,
Left, Up, Right, Down, //arrow keys
Select, Print, Execute, PrintScreen, Snapshot, Insert, Delete, Help,
D0, D1, D2, D3, D4, D5, D6, D7, D8, D9, //the number row on the keyboard
A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z,
LWin, RWin, Apps, Sleep,
NumPad0, NumPad1, NumPad2, NumPad3, NumPad4, NumPad5, NumPad6, NumPad7, NumPad8, NumPad9, Multiply, Add, Separator, Subtract, Decimal, Divide,
F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12, F13, F14, F15, F16, F17, F18, F19, F20, F21, F22, F23, F24, NumLock, Scroll,
LeftShift, RightShift, LeftCtrl, RightCtrl, LeftAlt, RightAlt,
BrowserBack, BrowserForward, BrowserRefresh, BrowserStop, BrowserSearch, BrowserFavorites, BrowserHome, VolumeMute, VolumeDown, VolumeUp, MediaNextTrack, MediaPreviousTrack, MediaStop, MediaPlayPause, LaunchMail, SelectMedia, LaunchApplication1, LaunchApplication2, Oem1, OemSemicolon, OemPlus, OemComma, OemMinus, OemPeriod, Oem2, OemQuestion, Oem3, OemTilde, AbntC1, AbntC2, Oem4, OemOpenBrackets, Oem5, OemPipe, Oem6, OemCloseBrackets, Oem7, OemQuotes, Oem8, Oem102, OemBackslash, ImeProcessed, System, DbeAlphanumeric, OemAttn, DbeKatakana, OemFinish, DbeHiragana, OemCopy, DbeSbcsChar, OemAuto, DbeDbcsChar, OemEnlw, DbeRoman, OemBackTab, Attn, DbeNoRoman, CrSel, DbeEnterWordRegisterMode, DbeEnterImeConfigureMode, ExSel, DbeFlushString, EraseEof, DbeCodeInput, Play, DbeNoCodeInput, Zoom, DbeDetermineString, NoName, DbeEnterDialogConversionMode, Pa1, OemClear, DeadCharProcessed
```

Mouse:
```c
LeftMouseButton, MiddleMouseButton, RightMouseButton, XButton1/*Back Button*/, XButton2 /*Forward Button*/
ScrollUp, ScrollRight, ScrollDown, ScrollLeft
```

Controller:
```c
XInputDPadUp, XInputDPadDown, XInputDPadLeft, XInputDPadRight, XInputDPadUpLeft, XInputDPadUpRight, XInputDPadDownLeft, XInputDPadDownRight,
XInputStart, XInputBack,
XInputLeftThumb, XInputRightThumb,
XInputLeftShoulder, XInputRightShoulder, XInputRT, XInputLT,
XInputA, XInputB, XInputX, XInputY,
XInputLStickUp, XInputLStickUpRight, XInputLStickRight, XInputLStickDownRight, XInputLStickDown, XInputLStickDownLeft, XInputLStickLeft, XInputLStickUpLeft,
XInputRStickUp, XInputRStickUpRight, XInputRStickRight, XInputRStickDownRight, XInputRStickDown, XInputRStickDownLeft, XInputRStickLeft, XInputRStickUpLeft
```