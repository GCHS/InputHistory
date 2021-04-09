﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Microsoft.Windows.Sdk;

namespace InputHistory {
	class RawInputHandler : IDisposable {
		enum UsagePage : ushort {//from Windows header hidusage.h
			UNDEFINED = 0x00,
			GENERIC = 0x01,
			SIMULATION = 0x02,
			VR = 0x03,
			SPORT = 0x04,
			GAME = 0x05,
			GENERIC_DEVICE = 0x06,
			KEYBOARD = 0x07,
			LED = 0x08,
			BUTTON = 0x09,
			ORDINAL = 0x0A,
			TELEPHONY = 0x0B,
			CONSUMER = 0x0C,
			DIGITIZER = 0x0D,
			HAPTICS = 0x0E,
			PID = 0x0F,
			UNICODE = 0x10,
			ALPHANUMERIC = 0x14,
			SENSOR = 0x20,
			LIGHTING_ILLUMINATION = 0x59,
			BARCODE_SCANNER = 0x8C,
			WEIGHING_DEVICE = 0x8D,
			MAGNETIC_STRIPE_READER = 0x8E,
			CAMERA_CONTROL = 0x90,
			ARCADE = 0x91,
			MICROSOFT_BLUETOOTH_HANDSFREE = 0xFFF3,
			VENDOR_DEFINED_BEGIN = 0xFF00,
			VENDOR_DEFINED_END = 0xFFFF,
		}
		enum UsageID {
			PAGE_UNDEFINED                        = 0x00,
			PAGE_GENERIC                          = 0x01,
			PAGE_SIMULATION                       = 0x02,
			PAGE_VR                               = 0x03,
			PAGE_SPORT                            = 0x04,
			PAGE_GAME                             = 0x05,
			PAGE_GENERIC_DEVICE                   = 0x06,
			PAGE_KEYBOARD                         = 0x07,
			PAGE_LED                              = 0x08,
			PAGE_BUTTON                           = 0x09,
			PAGE_ORDINAL                          = 0x0A,
			PAGE_TELEPHONY                        = 0x0B,
			PAGE_CONSUMER                         = 0x0C,
			PAGE_DIGITIZER                        = 0x0D,
			PAGE_HAPTICS                          = 0x0E,
			PAGE_PID                              = 0x0F,
			PAGE_UNICODE                          = 0x10,
			PAGE_ALPHANUMERIC                     = 0x14,
			PAGE_SENSOR                           = 0x20,
			PAGE_LIGHTING_ILLUMINATION            = 0x59,
			PAGE_BARCODE_SCANNER                  = 0x8C,
			PAGE_WEIGHING_DEVICE                  = 0x8D,
			PAGE_MAGNETIC_STRIPE_READER           = 0x8E,
			PAGE_CAMERA_CONTROL                   = 0x90,
			PAGE_ARCADE                           = 0x91,
			PAGE_MICROSOFT_BLUETOOTH_HANDSFREE    = 0xFFF3,
			PAGE_VENDOR_DEFINED_BEGIN             = 0xFF00,
			PAGE_VENDOR_DEFINED_END               = 0xFFFF,

			//
			// Generic Desktop Page (0x01,
			//
			GENERIC_POINTER                                       = 0x01,
			GENERIC_MOUSE                                         = 0x02,
			GENERIC_JOYSTICK                                      = 0x04,
			GENERIC_GAMEPAD                                       = 0x05,
			GENERIC_KEYBOARD                                      = 0x06,
			GENERIC_KEYPAD                                        = 0x07,
			GENERIC_MULTI_AXIS_CONTROLLER                         = 0x08,
			GENERIC_TABLET_PC_SYSTEM_CTL                          = 0x09,
			GENERIC_PORTABLE_DEVICE_CONTROL                       = 0x0D,
			GENERIC_INTERACTIVE_CONTROL                           = 0x0E,
			GENERIC_COUNTED_BUFFER                                = 0x3A,
			GENERIC_SYSTEM_CTL                                    = 0x80,

			GENERIC_X                                             = 0x30,
			GENERIC_Y                                             = 0x31,
			GENERIC_Z                                             = 0x32,
			GENERIC_RX                                            = 0x33,
			GENERIC_RY                                            = 0x34,
			GENERIC_RZ                                            = 0x35,
			GENERIC_SLIDER                                        = 0x36,
			GENERIC_DIAL                                          = 0x37,
			GENERIC_WHEEL                                         = 0x38,
			GENERIC_HATSWITCH                                     = 0x39,
			GENERIC_BYTE_COUNT                                    = 0x3B,
			GENERIC_MOTION_WAKEUP                                 = 0x3C,
			GENERIC_START                                         = 0x3D,
			GENERIC_SELECT                                        = 0x3E,
			GENERIC_VX                                            = 0x40,
			GENERIC_VY                                            = 0x41,
			GENERIC_VZ                                            = 0x42,
			GENERIC_VBRX                                          = 0x43,
			GENERIC_VBRY                                          = 0x44,
			GENERIC_VBRZ                                          = 0x45,
			GENERIC_VNO                                           = 0x46,
			GENERIC_FEATURE_NOTIFICATION                          = 0x47,
			GENERIC_RESOLUTION_MULTIPLIER                         = 0x48,
			GENERIC_SYSCTL_POWER                                  = 0x81,
			GENERIC_SYSCTL_SLEEP                                  = 0x82,
			GENERIC_SYSCTL_WAKE                                   = 0x83,
			GENERIC_SYSCTL_CONTEXT_MENU                           = 0x84,
			GENERIC_SYSCTL_MAIN_MENU                              = 0x85,
			GENERIC_SYSCTL_APP_MENU                               = 0x86,
			GENERIC_SYSCTL_HELP_MENU                              = 0x87,
			GENERIC_SYSCTL_MENU_EXIT                              = 0x88,
			GENERIC_SYSCTL_MENU_SELECT                            = 0x89,
			GENERIC_SYSCTL_MENU_RIGHT                             = 0x8A,
			GENERIC_SYSCTL_MENU_LEFT                              = 0x8B,
			GENERIC_SYSCTL_MENU_UP                                = 0x8C,
			GENERIC_SYSCTL_MENU_DOWN                              = 0x8D,
			GENERIC_SYSCTL_COLD_RESTART                           = 0x8E,
			GENERIC_SYSCTL_WARM_RESTART                           = 0x8F,
			GENERIC_DPAD_UP                                       = 0x90,
			GENERIC_DPAD_DOWN                                     = 0x91,
			GENERIC_DPAD_RIGHT                                    = 0x92,
			GENERIC_DPAD_LEFT                                     = 0x93,
			GENERIC_SYSCTL_FN                                     = 0x97,
			GENERIC_SYSCTL_FN_LOCK                                = 0x98,
			GENERIC_SYSCTL_FN_LOCK_INDICATOR                      = 0x99,
			GENERIC_SYSCTL_DISMISS_NOTIFICATION                   = 0x9A,
			GENERIC_SYSCTL_DOCK                                   = 0xA0,
			GENERIC_SYSCTL_UNDOCK                                 = 0xA1,
			GENERIC_SYSCTL_SETUP                                  = 0xA2,
			GENERIC_SYSCTL_SYS_BREAK                              = 0xA3,
			GENERIC_SYSCTL_SYS_DBG_BREAK                          = 0xA4,
			GENERIC_SYSCTL_APP_BREAK                              = 0xA5,
			GENERIC_SYSCTL_APP_DBG_BREAK                          = 0xA6,
			GENERIC_SYSCTL_MUTE                                   = 0xA7,
			GENERIC_SYSCTL_HIBERNATE                              = 0xA8,
			GENERIC_SYSCTL_DISP_INVERT                            = 0xB0,
			GENERIC_SYSCTL_DISP_INTERNAL                          = 0xB1,
			GENERIC_SYSCTL_DISP_EXTERNAL                          = 0xB2,
			GENERIC_SYSCTL_DISP_BOTH                              = 0xB3,
			GENERIC_SYSCTL_DISP_DUAL                              = 0xB4,
			GENERIC_SYSCTL_DISP_TOGGLE                            = 0xB5,
			GENERIC_SYSCTL_DISP_SWAP                              = 0xB6,
			GENERIC_SYSCTL_DISP_AUTOSCALE                         = 0xB7,
			GENERIC_SYSTEM_DISPLAY_ROTATION_LOCK_BUTTON           = 0xC9,
			GENERIC_SYSTEM_DISPLAY_ROTATION_LOCK_SLIDER_SWITCH    = 0xCA,
			GENERIC_CONTROL_ENABLE                                = 0xCB,

			//
			// Simulation Controls Page (0x02,
			//
			SIMULATION_FLIGHT_SIMULATION_DEVICE          = 0x01,
			SIMULATION_AUTOMOBILE_SIMULATION_DEVICE      = 0x02,
			SIMULATION_TANK_SIMULATION_DEVICE            = 0x03,
			SIMULATION_SPACESHIP_SIMULATION_DEVICE       = 0x04,
			SIMULATION_SUBMARINE_SIMULATION_DEVICE       = 0x05,
			SIMULATION_SAILING_SIMULATION_DEVICE         = 0x06,
			SIMULATION_MOTORCYCLE_SIMULATION_DEVICE      = 0x07,
			SIMULATION_SPORTS_SIMULATION_DEVICE          = 0x08,
			SIMULATION_AIRPLANE_SIMULATION_DEVICE        = 0x09,
			SIMULATION_HELICOPTER_SIMULATION_DEVICE      = 0x0A,
			SIMULATION_MAGIC_CARPET_SIMULATION_DEVICE    = 0x0B,
			SIMULATION_BICYCLE_SIMULATION_DEVICE         = 0x0C,
			SIMULATION_FLIGHT_CONTROL_STICK              = 0x20,
			SIMULATION_FLIGHT_STICK                      = 0x21,
			SIMULATION_CYCLIC_CONTROL                    = 0x22,
			SIMULATION_CYCLIC_TRIM                       = 0x23,
			SIMULATION_FLIGHT_YOKE                       = 0x24,
			SIMULATION_TRACK_CONTROL                     = 0x25,

			SIMULATION_AILERON                           = 0xB0,
			SIMULATION_AILERON_TRIM                      = 0xB1,
			SIMULATION_ANTI_TORQUE_CONTROL               = 0xB2,
			SIMULATION_AUTOPIOLOT_ENABLE                 = 0xB3,
			SIMULATION_CHAFF_RELEASE                     = 0xB4,
			SIMULATION_COLLECTIVE_CONTROL                = 0xB5,
			SIMULATION_DIVE_BRAKE                        = 0xB6,
			SIMULATION_ELECTRONIC_COUNTERMEASURES        = 0xB7,
			SIMULATION_ELEVATOR                          = 0xB8,
			SIMULATION_ELEVATOR_TRIM                     = 0xB9,
			SIMULATION_RUDDER                            = 0xBA,
			SIMULATION_THROTTLE                          = 0xBB,
			SIMULATION_FLIGHT_COMMUNICATIONS             = 0xBC,
			SIMULATION_FLARE_RELEASE                     = 0xBD,
			SIMULATION_LANDING_GEAR                      = 0xBE,
			SIMULATION_TOE_BRAKE                         = 0xBF,
			SIMULATION_TRIGGER                           = 0xC0,
			SIMULATION_WEAPONS_ARM                       = 0xC1,
			SIMULATION_WEAPONS_SELECT                    = 0xC2,
			SIMULATION_WING_FLAPS                        = 0xC3,
			SIMULATION_ACCELLERATOR                      = 0xC4,
			SIMULATION_BRAKE                             = 0xC5,
			SIMULATION_CLUTCH                            = 0xC6,
			SIMULATION_SHIFTER                           = 0xC7,
			SIMULATION_STEERING                          = 0xC8,
			SIMULATION_TURRET_DIRECTION                  = 0xC9,
			SIMULATION_BARREL_ELEVATION                  = 0xCA,
			SIMULATION_DIVE_PLANE                        = 0xCB,
			SIMULATION_BALLAST                           = 0xCC,
			SIMULATION_BICYCLE_CRANK                     = 0xCD,
			SIMULATION_HANDLE_BARS                       = 0xCE,
			SIMULATION_FRONT_BRAKE                       = 0xCF,
			SIMULATION_REAR_BRAKE                        = 0xD0,

			//
			// Virtual Reality Controls Page (0x03,
			//
			VR_BELT                    = 0x01,
			VR_BODY_SUIT               = 0x02,
			VR_FLEXOR                  = 0x03,
			VR_GLOVE                   = 0x04,
			VR_HEAD_TRACKER            = 0x05,
			VR_HEAD_MOUNTED_DISPLAY    = 0x06,
			VR_HAND_TRACKER            = 0x07,
			VR_OCULOMETER              = 0x08,
			VR_VEST                    = 0x09,
			VR_ANIMATRONIC_DEVICE      = 0x0A,

			VR_STEREO_ENABLE           = 0x20,
			VR_DISPLAY_ENABLE          = 0x21,

			//
			// Sport Controls Page (0x04,
			//
			SPORT_BASEBALL_BAT        = 0x01,
			SPORT_GOLF_CLUB           = 0x02,
			SPORT_ROWING_MACHINE      = 0x03,
			SPORT_TREADMILL           = 0x04,
			SPORT_STICK_TYPE          = 0x38,

			SPORT_OAR                 = 0x30,
			SPORT_SLOPE               = 0x31,
			SPORT_RATE                = 0x32,
			SPORT_STICK_SPEED         = 0x33,
			SPORT_STICK_FACE_ANGLE    = 0x34,
			SPORT_HEEL_TOE            = 0x35,
			SPORT_FOLLOW_THROUGH      = 0x36,
			SPORT_TEMPO               = 0x37,
			SPORT_HEIGHT              = 0x39,
			SPORT_PUTTER              = 0x50,
			SPORT_1_IRON              = 0x51,
			SPORT_2_IRON              = 0x52,
			SPORT_3_IRON              = 0x53,
			SPORT_4_IRON              = 0x54,
			SPORT_5_IRON              = 0x55,
			SPORT_6_IRON              = 0x56,
			SPORT_7_IRON              = 0x57,
			SPORT_8_IRON              = 0x58,
			SPORT_9_IRON              = 0x59,
			SPORT_10_IRON             = 0x5A,
			SPORT_11_IRON             = 0x5B,
			SPORT_SAND_WEDGE          = 0x5C,
			SPORT_LOFT_WEDGE          = 0x5D,
			SPORT_POWER_WEDGE         = 0x5E,
			SPORT_1_WOOD              = 0x5F,
			SPORT_3_WOOD              = 0x60,
			SPORT_5_WOOD              = 0x61,
			SPORT_7_WOOD              = 0x62,
			SPORT_9_WOOD              = 0x63,

			//
			// Game Controls Page (0x05,
			//
			GAME_3D_GAME_CONTROLLER    = 0x01,
			GAME_PINBALL_DEVICE        = 0x02,
			GAME_GUN_DEVICE            = 0x03,
			GAME_POINT_OF_VIEW         = 0x20,
			GAME_GUN_SELECTOR          = 0x32,
			GAME_GAMEPAD_FIRE_JUMP     = 0x37,
			GAME_GAMEPAD_TRIGGER       = 0x39,

			GAME_TURN_RIGHT_LEFT       = 0x21,
			GAME_PITCH_FORWARD_BACK    = 0x22,
			GAME_ROLL_RIGHT_LEFT       = 0x23,
			GAME_MOVE_RIGHT_LEFT       = 0x24,
			GAME_MOVE_FORWARD_BACK     = 0x25,
			GAME_MOVE_UP_DOWN          = 0x26,
			GAME_LEAN_RIGHT_LEFT       = 0x27,
			GAME_LEAN_FORWARD_BACK     = 0x28,
			GAME_POV_HEIGHT            = 0x29,
			GAME_FLIPPER               = 0x2A,
			GAME_SECONDARY_FLIPPER     = 0x2B,
			GAME_BUMP                  = 0x2C,
			GAME_NEW_GAME              = 0x2D,
			GAME_SHOOT_BALL            = 0x2E,
			GAME_PLAYER                = 0x2F,
			GAME_GUN_BOLT              = 0x30,
			GAME_GUN_CLIP              = 0x31,
			GAME_GUN_SINGLE_SHOT       = 0x33,
			GAME_GUN_BURST             = 0x34,
			GAME_GUN_AUTOMATIC         = 0x35,
			GAME_GUN_SAFETY            = 0x36,

			//
			// Generic Device Controls Page (0x06,
			//
			GENERIC_DEVICE_BATTERY_STRENGTH              = 0x20,
			GENERIC_DEVICE_WIRELESS_CHANNEL              = 0x21,
			GENERIC_DEVICE_WIRELESS_ID                   = 0x22,
			GENERIC_DEVICE_DISCOVER_WIRELESS_CONTROL     = 0x23,
			GENERIC_DEVICE_SECURITY_CODE_CHAR_ENTERED    = 0x24,
			GENERIC_DEVICE_SECURITY_CODE_CHAR_ERASED     = 0x25,
			GENERIC_DEVICE_SECURITY_CODE_CLEARED         = 0x26,

			//
			// Keyboard/Keypad Page (0x07,
			//

			// Error "keys"
			KEYBOARD_NOEVENT     = 0x00,
			KEYBOARD_ROLLOVER    = 0x01,
			KEYBOARD_POSTFAIL    = 0x02,
			KEYBOARD_UNDEFINED   = 0x03,

			// Letters
			KEYBOARD_aA          = 0x04,
			KEYBOARD_zZ          = 0x1D,

			// Numbers
			KEYBOARD_ONE         = 0x1E,
			KEYBOARD_ZERO        = 0x27,

			// Modifier Keys
			KEYBOARD_LCTRL       = 0xE0,
			KEYBOARD_LSHFT       = 0xE1,
			KEYBOARD_LALT        = 0xE2,
			KEYBOARD_LGUI        = 0xE3,
			KEYBOARD_RCTRL       = 0xE4,
			KEYBOARD_RSHFT       = 0xE5,
			KEYBOARD_RALT        = 0xE6,
			KEYBOARD_RGUI        = 0xE7,
			KEYBOARD_SCROLL_LOCK = 0x47,
			KEYBOARD_NUM_LOCK    = 0x53,
			KEYBOARD_CAPS_LOCK   = 0x39,

						// Function keys
			KEYBOARD_F1          = 0x3A,
			KEYBOARD_F2          = 0x3B,
			KEYBOARD_F3          = 0x3C,
			KEYBOARD_F4          = 0x3D,
			KEYBOARD_F5          = 0x3E,
			KEYBOARD_F6          = 0x3F,
			KEYBOARD_F7          = 0x40,
			KEYBOARD_F8          = 0x41,
			KEYBOARD_F9          = 0x42,
			KEYBOARD_F10         = 0x43,
			KEYBOARD_F11         = 0x44,
			KEYBOARD_F12         = 0x45,
			KEYBOARD_F13         = 0x68,
			KEYBOARD_F14         = 0x69,
			KEYBOARD_F15         = 0x6A,
			KEYBOARD_F16         = 0x6B,
			KEYBOARD_F17         = 0x6C,
			KEYBOARD_F18         = 0x6D,
			KEYBOARD_F19         = 0x6E,
			KEYBOARD_F20         = 0x6F,
			KEYBOARD_F21         = 0x70,
			KEYBOARD_F22         = 0x71,
			KEYBOARD_F23         = 0x72,
			KEYBOARD_F24         = 0x73,

			KEYBOARD_RETURN      = 0x28,
			KEYBOARD_ESCAPE      = 0x29,
			KEYBOARD_DELETE      = 0x2A,

			KEYBOARD_PRINT_SCREEN      = 0x46,
			KEYBOARD_DELETE_FORWARD    = 0x4C,


			//
			// LED Page (0x08,
			//
			LED_NUM_LOCK               = 0x01,
			LED_CAPS_LOCK              = 0x02,
			LED_SCROLL_LOCK            = 0x03,
			LED_COMPOSE                = 0x04,
			LED_KANA                   = 0x05,
			LED_POWER                  = 0x06,
			LED_SHIFT                  = 0x07,
			LED_DO_NOT_DISTURB         = 0x08,
			LED_MUTE                   = 0x09,
			LED_TONE_ENABLE            = 0x0A,
			LED_HIGH_CUT_FILTER        = 0x0B,
			LED_LOW_CUT_FILTER         = 0x0C,
			LED_EQUALIZER_ENABLE       = 0x0D,
			LED_SOUND_FIELD_ON         = 0x0E,
			LED_SURROUND_FIELD_ON      = 0x0F,
			LED_REPEAT                 = 0x10,
			LED_STEREO                 = 0x11,
			LED_SAMPLING_RATE_DETECT   = 0x12,
			LED_SPINNING               = 0x13,
			LED_CAV                    = 0x14,
			LED_CLV                    = 0x15,
			LED_RECORDING_FORMAT_DET   = 0x16,
			LED_OFF_HOOK               = 0x17,
			LED_RING                   = 0x18,
			LED_MESSAGE_WAITING        = 0x19,
			LED_DATA_MODE              = 0x1A,
			LED_BATTERY_OPERATION      = 0x1B,
			LED_BATTERY_OK             = 0x1C,
			LED_BATTERY_LOW            = 0x1D,
			LED_SPEAKER                = 0x1E,
			LED_HEAD_SET               = 0x1F,
			LED_HOLD                   = 0x20,
			LED_MICROPHONE             = 0x21,
			LED_COVERAGE               = 0x22,
			LED_NIGHT_MODE             = 0x23,
			LED_SEND_CALLS             = 0x24,
			LED_CALL_PICKUP            = 0x25,
			LED_CONFERENCE             = 0x26,
			LED_STAND_BY               = 0x27,
			LED_CAMERA_ON              = 0x28,
			LED_CAMERA_OFF             = 0x29,
			LED_ON_LINE                = 0x2A,
			LED_OFF_LINE               = 0x2B,
			LED_BUSY                   = 0x2C,
			LED_READY                  = 0x2D,
			LED_PAPER_OUT              = 0x2E,
			LED_PAPER_JAM              = 0x2F,
			LED_REMOTE                 = 0x30,
			LED_FORWARD                = 0x31,
			LED_REVERSE                = 0x32,
			LED_STOP                   = 0x33,
			LED_REWIND                 = 0x34,
			LED_FAST_FORWARD           = 0x35,
			LED_PLAY                   = 0x36,
			LED_PAUSE                  = 0x37,
			LED_RECORD                 = 0x38,
			LED_ERROR                  = 0x39,
			LED_SELECTED_INDICATOR     = 0x3A,
			LED_IN_USE_INDICATOR       = 0x3B,
			LED_MULTI_MODE_INDICATOR   = 0x3C,
			LED_INDICATOR_ON           = 0x3D,
			LED_INDICATOR_FLASH        = 0x3E,
			LED_INDICATOR_SLOW_BLINK   = 0x3F,
			LED_INDICATOR_FAST_BLINK   = 0x40,
			LED_INDICATOR_OFF          = 0x41,
			LED_FLASH_ON_TIME          = 0x42,
			LED_SLOW_BLINK_ON_TIME     = 0x43,
			LED_SLOW_BLINK_OFF_TIME    = 0x44,
			LED_FAST_BLINK_ON_TIME     = 0x45,
			LED_FAST_BLINK_OFF_TIME    = 0x46,
			LED_INDICATOR_COLOR        = 0x47,
			LED_RED                    = 0x48,
			LED_GREEN                  = 0x49,
			LED_AMBER                  = 0x4A,
			LED_GENERIC_INDICATOR      = 0x4B,
			LED_SYSTEM_SUSPEND         = 0x4C,
			LED_EXTERNAL_POWER         = 0x4D,

			//
			//  Button Page (0x09,
			//
			//  There is no need to label these usages.
			//


			//
			//  Ordinal Page (0x0A,
			//
			//  There is no need to label these usages.
			//


			//
			//  Telephony Device Page (0x0B,
			//
			TELEPHONY_PHONE                   = 0x01,
			TELEPHONY_ANSWERING_MACHINE       = 0x02,
			TELEPHONY_MESSAGE_CONTROLS        = 0x03,
			TELEPHONY_HANDSET                 = 0x04,
			TELEPHONY_HEADSET                 = 0x05,
			TELEPHONY_KEYPAD                  = 0x06,
			TELEPHONY_PROGRAMMABLE_BUTTON     = 0x07,
			TELEPHONY_REDIAL                  = 0x24,
			TELEPHONY_TRANSFER                = 0x25,
			TELEPHONY_DROP                    = 0x26,
			TELEPHONY_LINE                    = 0x2A,
			TELEPHONY_RING_ENABLE             = 0x2D,
			TELEPHONY_SEND                    = 0x31,
			TELEPHONY_KEYPAD_0                = 0xB0,
			TELEPHONY_KEYPAD_D                = 0xBF,
			TELEPHONY_HOST_AVAILABLE          = 0xF1,


			//
			// Consumer Controls Page (0x0C,
			//
			CONSUMERCTRL                      = 0x01,

			// channel
			CONSUMER_CHANNEL_INCREMENT        = 0x9C,
			CONSUMER_CHANNEL_DECREMENT        = 0x9D,

			// transport control
			CONSUMER_PLAY                     = 0xB0,
			CONSUMER_PAUSE                    = 0xB1,
			CONSUMER_RECORD                   = 0xB2,
			CONSUMER_FAST_FORWARD             = 0xB3,
			CONSUMER_REWIND                   = 0xB4,
			CONSUMER_SCAN_NEXT_TRACK          = 0xB5,
			CONSUMER_SCAN_PREV_TRACK          = 0xB6,
			CONSUMER_STOP                     = 0xB7,
			CONSUMER_PLAY_PAUSE               = 0xCD,

			// GameDVR
			CONSUMER_GAMEDVR_OPEN_GAMEBAR     = 0xD0,
			CONSUMER_GAMEDVR_TOGGLE_RECORD    = 0xD1,
			CONSUMER_GAMEDVR_RECORD_CLIP      = 0xD2,
			CONSUMER_GAMEDVR_SCREENSHOT       = 0xD3,
			CONSUMER_GAMEDVR_TOGGLE_INDICATOR = 0xD4,
			CONSUMER_GAMEDVR_TOGGLE_MICROPHONE = 0xD5,
			CONSUMER_GAMEDVR_TOGGLE_CAMERA    = 0xD6,
			CONSUMER_GAMEDVR_TOGGLE_BROADCAST = 0xD7,

			// audio
			CONSUMER_VOLUME                   = 0xE0,
			CONSUMER_BALANCE                  = 0xE1,
			CONSUMER_MUTE                     = 0xE2,
			CONSUMER_BASS                     = 0xE3,
			CONSUMER_TREBLE                   = 0xE4,
			CONSUMER_BASS_BOOST               = 0xE5,
			CONSUMER_SURROUND_MODE            = 0xE6,
			CONSUMER_LOUDNESS                 = 0xE7,
			CONSUMER_MPX                      = 0xE8,
			CONSUMER_VOLUME_INCREMENT         = 0xE9,
			CONSUMER_VOLUME_DECREMENT         = 0xEA,

			// supplementary audio
			CONSUMER_BASS_INCREMENT           = 0x152,
			CONSUMER_BASS_DECREMENT           = 0x153,
			CONSUMER_TREBLE_INCREMENT         = 0x154,
			CONSUMER_TREBLE_DECREMENT         = 0x155,

			// Application Launch
			CONSUMER_AL_CONFIGURATION         = 0x183,
			CONSUMER_AL_EMAIL                 = 0x18A,
			CONSUMER_AL_CALCULATOR            = 0x192,
			CONSUMER_AL_BROWSER               = 0x194,
			CONSUMER_AL_SEARCH                = 0x1C6,

			// Application Control
			CONSUMER_AC_SEARCH                = 0x221,
			CONSUMER_AC_GOTO                  = 0x222,
			CONSUMER_AC_HOME                  = 0x223,
			CONSUMER_AC_BACK                  = 0x224,
			CONSUMER_AC_FORWARD               = 0x225,
			CONSUMER_AC_STOP                  = 0x226,
			CONSUMER_AC_REFRESH               = 0x227,
			CONSUMER_AC_PREVIOUS              = 0x228,
			CONSUMER_AC_NEXT                  = 0x229,
			CONSUMER_AC_BOOKMARKS             = 0x22A,
			CONSUMER_AC_PAN                   = 0x238,

			// Keyboard Extended Attributes (defined on consumer page in HUTRR42,
			CONSUMER_EXTENDED_KEYBOARD_ATTRIBUTES_COLLECTION      = 0x2C0,
			CONSUMER_KEYBOARD_FORM_FACTOR                         = 0x2C1,
			CONSUMER_KEYBOARD_KEY_TYPE                            = 0x2C2,
			CONSUMER_KEYBOARD_PHYSICAL_LAYOUT                     = 0x2C3,
			CONSUMER_VENDOR_SPECIFIC_KEYBOARD_PHYSICAL_LAYOUT     = 0x2C4,
			CONSUMER_KEYBOARD_IETF_LANGUAGE_TAG_INDEX             = 0x2C5,
			CONSUMER_IMPLEMENTED_KEYBOARD_INPUT_ASSIST_CONTROLS   = 0x2C6,

			//
			// Digitizer Page (0x0D,
			//
			DIGITIZER_DIGITIZER               = 0x01,
			DIGITIZER_PEN                     = 0x02,
			DIGITIZER_LIGHT_PEN               = 0x03,
			DIGITIZER_TOUCH_SCREEN            = 0x04,
			DIGITIZER_TOUCH_PAD               = 0x05,
			DIGITIZER_WHITE_BOARD             = 0x06,
			DIGITIZER_COORD_MEASURING         = 0x07,
			DIGITIZER_3D_DIGITIZER            = 0x08,
			DIGITIZER_STEREO_PLOTTER          = 0x09,
			DIGITIZER_ARTICULATED_ARM         = 0x0A,
			DIGITIZER_ARMATURE                = 0x0B,
			DIGITIZER_MULTI_POINT             = 0x0C,
			DIGITIZER_FREE_SPACE_WAND         = 0x0D,
			DIGITIZER_STYLUS                  = 0x20,
			DIGITIZER_PUCK                    = 0x21,
			DIGITIZER_FINGER                  = 0x22,
			DIGITIZER_TABLET_FUNC_KEYS        = 0x39,
			DIGITIZER_PROG_CHANGE_KEYS        = 0x3A,

			DIGITIZER_TIP_PRESSURE            = 0x30,
			DIGITIZER_BARREL_PRESSURE         = 0x31,
			DIGITIZER_IN_RANGE                = 0x32,
			DIGITIZER_TOUCH                   = 0x33,
			DIGITIZER_UNTOUCH                 = 0x34,
			DIGITIZER_TAP                     = 0x35,
			DIGITIZER_QUALITY                 = 0x36,
			DIGITIZER_DATA_VALID              = 0x37,
			DIGITIZER_TRANSDUCER_INDEX        = 0x38,
			DIGITIZER_BATTERY_STRENGTH        = 0x3B,
			DIGITIZER_INVERT                  = 0x3C,
			DIGITIZER_X_TILT                  = 0x3D,
			DIGITIZER_Y_TILT                  = 0x3E,
			DIGITIZER_AZIMUTH                 = 0x3F,
			DIGITIZER_ALTITUDE                = 0x40,
			DIGITIZER_TWIST                   = 0x41,
			DIGITIZER_TIP_SWITCH              = 0x42,
			DIGITIZER_SECONDARY_TIP_SWITCH    = 0x43,
			DIGITIZER_BARREL_SWITCH           = 0x44,
			DIGITIZER_ERASER                  = 0x45,
			DIGITIZER_TABLET_PICK             = 0x46,
			DIGITIZER_TRANSDUCER_SERIAL       = 0x5B,
			DIGITIZER_TRANSDUCER_VENDOR       = 0x92,
			DIGITIZER_TRANSDUCER_CONNECTED    = 0xA2,

			//
			// Simple Haptic Controller Page (0x0E,
			//
			HAPTICS_SIMPLE_CONTROLLER         =0x01,

			HAPTICS_WAVEFORM_LIST             =0x10,
			HAPTICS_DURATION_LIST             =0x11,

			HAPTICS_AUTO_TRIGGER              =0x20,
			HAPTICS_MANUAL_TRIGGER            =0x21,
			HAPTICS_AUTO_ASSOCIATED_CONTROL   =0x22,
			HAPTICS_INTENSITY                 =0x23,
			HAPTICS_REPEAT_COUNT              =0x24,
			HAPTICS_RETRIGGER_PERIOD          =0x25,
			HAPTICS_WAVEFORM_VENDOR_PAGE      =0x26,
			HAPTICS_WAVEFORM_VENDOR_ID        =0x27,
			HAPTICS_WAVEFORM_CUTOFF_TIME      =0x28,

			// Waveform types
			HAPTICS_WAVEFORM_BEGIN            =0x1000,
			HAPTICS_WAVEFORM_STOP             =0x1001,
			HAPTICS_WAVEFORM_NULL             =0x1002,
			HAPTICS_WAVEFORM_CLICK            =0x1003,
			HAPTICS_WAVEFORM_BUZZ             =0x1004,
			HAPTICS_WAVEFORM_RUMBLE           =0x1005,
			HAPTICS_WAVEFORM_PRESS            =0x1006,
			HAPTICS_WAVEFORM_RELEASE          =0x1007,
			HAPTICS_WAVEFORM_END              =0x1FFF,

			HAPTICS_WAVEFORM_VENDOR_BEGIN     =0x2000,
			HAPTICS_WAVEFORM_VENDOR_END       =0x2FFF,

			//
			//  Unicode Page (0x10,
			//
			//  There is no need to label these usages.
			//

			//
			//  Alphanumeric Display Page (0x14,
			//
			ALPHANUMERIC_ALPHANUMERIC_DISPLAY            = 0x01,
			ALPHANUMERIC_BITMAPPED_DISPLAY               = 0x02,
			ALPHANUMERIC_DISPLAY_ATTRIBUTES_REPORT       = 0x20,
			ALPHANUMERIC_DISPLAY_CONTROL_REPORT          = 0x24,
			ALPHANUMERIC_CHARACTER_REPORT                = 0x2B,
			ALPHANUMERIC_DISPLAY_STATUS                  = 0x2D,
			ALPHANUMERIC_CURSOR_POSITION_REPORT          = 0x32,
			ALPHANUMERIC_FONT_REPORT                     = 0x3B,
			ALPHANUMERIC_FONT_DATA                       = 0x3C,
			ALPHANUMERIC_CHARACTER_ATTRIBUTE             = 0x48,
			ALPHANUMERIC_PALETTE_REPORT                  = 0x85,
			ALPHANUMERIC_PALETTE_DATA                    = 0x88,
			ALPHANUMERIC_BLIT_REPORT                     = 0x8A,
			ALPHANUMERIC_BLIT_DATA                       = 0x8F,
			ALPHANUMERIC_SOFT_BUTTON                     = 0x90,

			ALPHANUMERIC_ASCII_CHARACTER_SET             = 0x21,
			ALPHANUMERIC_DATA_READ_BACK                  = 0x22,
			ALPHANUMERIC_FONT_READ_BACK                  = 0x23,
			ALPHANUMERIC_CLEAR_DISPLAY                   = 0x25,
			ALPHANUMERIC_DISPLAY_ENABLE                  = 0x26,
			ALPHANUMERIC_SCREEN_SAVER_DELAY              = 0x27,
			ALPHANUMERIC_SCREEN_SAVER_ENABLE             = 0x28,
			ALPHANUMERIC_VERTICAL_SCROLL                 = 0x29,
			ALPHANUMERIC_HORIZONTAL_SCROLL               = 0x2A,
			ALPHANUMERIC_DISPLAY_DATA                    = 0x2C,
			ALPHANUMERIC_STATUS_NOT_READY                = 0x2E,
			ALPHANUMERIC_STATUS_READY                    = 0x2F,
			ALPHANUMERIC_ERR_NOT_A_LOADABLE_CHARACTER    = 0x30,
			ALPHANUMERIC_ERR_FONT_DATA_CANNOT_BE_READ    = 0x31,
			ALPHANUMERIC_ROW                             = 0x33,
			ALPHANUMERIC_COLUMN                          = 0x34,
			ALPHANUMERIC_ROWS                            = 0x35,
			ALPHANUMERIC_COLUMNS                         = 0x36,
			ALPHANUMERIC_CURSOR_PIXEL_POSITIONING        = 0x37,
			ALPHANUMERIC_CURSOR_MODE                     = 0x38,
			ALPHANUMERIC_CURSOR_ENABLE                   = 0x39,
			ALPHANUMERIC_CURSOR_BLINK                    = 0x3A,
			ALPHANUMERIC_CHAR_WIDTH                      = 0x3D,
			ALPHANUMERIC_CHAR_HEIGHT                     = 0x3E,
			ALPHANUMERIC_CHAR_SPACING_HORIZONTAL         = 0x3F,
			ALPHANUMERIC_CHAR_SPACING_VERTICAL           = 0x40,
			ALPHANUMERIC_UNICODE_CHAR_SET                = 0x41,
			ALPHANUMERIC_FONT_7_SEGMENT                  = 0x42,
			ALPHANUMERIC_7_SEGMENT_DIRECT_MAP            = 0x43,
			ALPHANUMERIC_FONT_14_SEGMENT                 = 0x44,
			ALPHANUMERIC_14_SEGMENT_DIRECT_MAP           = 0x45,
			ALPHANUMERIC_DISPLAY_BRIGHTNESS              = 0x46,
			ALPHANUMERIC_DISPLAY_CONTRAST                = 0x47,
			ALPHANUMERIC_ATTRIBUTE_READBACK              = 0x49,
			ALPHANUMERIC_ATTRIBUTE_DATA                  = 0x4A,
			ALPHANUMERIC_CHAR_ATTR_ENHANCE               = 0x4B,
			ALPHANUMERIC_CHAR_ATTR_UNDERLINE             = 0x4C,
			ALPHANUMERIC_CHAR_ATTR_BLINK                 = 0x4D,
			ALPHANUMERIC_BITMAP_SIZE_X                   = 0x80,
			ALPHANUMERIC_BITMAP_SIZE_Y                   = 0x81,
			ALPHANUMERIC_BIT_DEPTH_FORMAT                = 0x83,
			ALPHANUMERIC_DISPLAY_ORIENTATION             = 0x84,
			ALPHANUMERIC_PALETTE_DATA_SIZE               = 0x86,
			ALPHANUMERIC_PALETTE_DATA_OFFSET             = 0x87,
			ALPHANUMERIC_BLIT_RECTANGLE_X1               = 0x8B,
			ALPHANUMERIC_BLIT_RECTANGLE_Y1               = 0x8C,
			ALPHANUMERIC_BLIT_RECTANGLE_X2               = 0x8D,
			ALPHANUMERIC_BLIT_RECTANGLE_Y2               = 0x8E,
			ALPHANUMERIC_SOFT_BUTTON_ID                  = 0x91,
			ALPHANUMERIC_SOFT_BUTTON_SIDE                = 0x92,
			ALPHANUMERIC_SOFT_BUTTON_OFFSET1             = 0x93,
			ALPHANUMERIC_SOFT_BUTTON_OFFSET2             = 0x94,
			ALPHANUMERIC_SOFT_BUTTON_REPORT              = 0x95,

			//
			// Sensor Page (0x20,
			//

			//
			// LampArray Page (0x59,
			//
			LAMPARRAY                                             = 0x01,
			LAMPARRAY_ATTRBIUTES_REPORT                           = 0x02,
			LAMPARRAY_LAMP_COUNT                                  = 0x03,
			LAMPARRAY_BOUNDING_BOX_WIDTH_IN_MICROMETERS           = 0x04,
			LAMPARRAY_BOUNDING_BOX_HEIGHT_IN_MICROMETERS          = 0x05,
			LAMPARRAY_BOUNDING_BOX_DEPTH_IN_MICROMETERS           = 0x06,
			LAMPARRAY_KIND                                        = 0x07,
			LAMPARRAY_MIN_UPDATE_INTERVAL_IN_MICROSECONDS         = 0x08,

			// 0x09 - 0x1F Reserved

			LAMPARRAY_LAMP_ATTRIBUTES_REQUEST_REPORT              = 0x20,
			LAMPARRAY_LAMP_ID                                     = 0x21,
			LAMPARRAY_LAMP_ATTRIBUTES_RESPONSE_REPORT             = 0x22,
			LAMPARRAY_POSITION_X_IN_MICROMETERS                   = 0x23,
			LAMPARRAY_POSITION_Y_IN_MICROMETERS                   = 0x24,
			LAMPARRAY_POSITION_Z_IN_MICROMETERS                   = 0x25,
			LAMPARRAY_LAMP_PURPOSES                               = 0x26,
			LAMPARRAY_UPDATE_LATENCY_IN_MICROSECONDS              = 0x27,
			LAMPARRAY_RED_LEVEL_COUNT                             = 0x28,
			LAMPARRAY_GREEN_LEVEL_COUNT                           = 0x29,
			LAMPARRAY_BLUE_LEVEL_COUNT                            = 0x2A,
			LAMPARRAY_INTENSITY_LEVEL_COUNT                       = 0x2B,
			LAMPARRAY_IS_PROGRAMMABLE                             = 0x2C,
			LAMPARRAY_INPUT_BINDING                               = 0x2D,

			// 0x2E - 0x4F Reserved

			LAMPARRAY_LAMP_MULTI_UPDATE_REPORT                    = 0x50,
			LAMPARRAY_LAMP_RED_UPDATE_CHANNEL                     = 0x51,
			LAMPARRAY_LAMP_GREEN_UPDATE_CHANNEL                   = 0x52,
			LAMPARRAY_LAMP_BLUE_UPDATE_CHANNEL                    = 0x53,
			LAMPARRAY_LAMP_INTENSITY_UPDATE_CHANNEL               = 0x54,
			LAMPARRAY_LAMP_UPDATE_FLAGS                           = 0x55,

			// 0x55 - 0x5F Reserved

			LAMPARRAY_LAMP_RANGE_UPDATE_REPORT                    = 0x60,
			LAMPARRAY_LAMP_ID_START                               = 0x61,
			LAMPARRAY_LAMP_ID_END                                 = 0x62,

						// 0x63 - 0x6F Reserved

			LAMPARRAY_CONTROL_REPORT                              = 0x70,
			LAMPARRAY_AUTONOMOUS_MODE                             = 0x71,

			//
			// Camera Control Page (0x90,
			//
			CAMERA_AUTO_FOCUS                 = 0x20,
			CAMERA_SHUTTER                    = 0x21,

			//
			// Microsoft Bluetooth Handsfree Page (0xFFF3,
			//
			MS_BTH_HF_DIALNUMBER              = 0x21,
			MS_BTH_HF_DIALMEMORY              = 0x22,
		}
		enum DeviceType : int {
			MOUSE = 0,
			KEYBOARD = 1,
			HID = 2 //other
		}
		[Flags]
		enum MouseEventFlags : ushort {
			NONE               = 0x0000,
			LEFT_BUTTON_DOWN   = 0x0001,  // Left Button changed to down.
			LEFT_BUTTON_UP     = 0x0002,  // Left Button changed to up.
			RIGHT_BUTTON_DOWN  = 0x0004,  // Right Button changed to down.
			RIGHT_BUTTON_UP    = 0x0008,  // Right Button changed to up.
			MIDDLE_BUTTON_DOWN = 0x0010,  // Middle Button changed to down.
			MIDDLE_BUTTON_UP   = 0x0020,  // Middle Button changed to up.
			
			BUTTON_1_DOWN      = LEFT_BUTTON_DOWN	 ,
			BUTTON_1_UP        = LEFT_BUTTON_UP		 ,
			BUTTON_2_DOWN      = RIGHT_BUTTON_DOWN ,
			BUTTON_2_UP        = RIGHT_BUTTON_UP	 ,
			BUTTON_3_DOWN      = MIDDLE_BUTTON_DOWN,
			BUTTON_3_UP        = MIDDLE_BUTTON_UP  ,
			
			BUTTON_4_DOWN      = 0x0040,
			BUTTON_4_UP        = 0x0080,
			BUTTON_5_DOWN      = 0x0100,
			BUTTON_5_UP        = 0x0200,
			
			WHEEL  = 0x0400, //If usButtonFlags has WHEEL, the wheel delta is stored in usButtonData. Take it as a signed value.
			HWHEEL = 0x0800
		}

		private enum Direction {
			Up, Down
		}

		private readonly RAWINPUTDEVICE[] DevicesToRegisterFor = new RAWINPUTDEVICE[] {
					new(){
						usUsagePage = (ushort) UsagePage.GENERIC,
						usUsage = (ushort)UsageID.GENERIC_KEYBOARD,
					},
					new(){
						usUsagePage = (ushort) UsagePage.GENERIC,
						usUsage = (ushort)UsageID.GENERIC_MOUSE,
					},
					new() {
						usUsagePage = (ushort)UsagePage.GENERIC,
						usUsage = (ushort)UsageID.GENERIC_GAMEPAD,
					},
				};

		private readonly (MouseEventFlags, MouseButton, Direction)[] FlagToButton = new (MouseEventFlags, MouseButton, Direction)[] {
			(MouseEventFlags.LEFT_BUTTON_DOWN  , MouseButton.Left    , Direction.Down),
			(MouseEventFlags.LEFT_BUTTON_UP    , MouseButton.Left    , Direction.Up),
			(MouseEventFlags.RIGHT_BUTTON_DOWN , MouseButton.Right   , Direction.Down),
			(MouseEventFlags.RIGHT_BUTTON_UP   , MouseButton.Right   , Direction.Up),
			(MouseEventFlags.MIDDLE_BUTTON_DOWN, MouseButton.Middle  , Direction.Down),
			(MouseEventFlags.MIDDLE_BUTTON_UP  , MouseButton.Middle  , Direction.Up),
			(MouseEventFlags.BUTTON_4_DOWN     , MouseButton.XButton1, Direction.Down),
			(MouseEventFlags.BUTTON_4_UP       , MouseButton.XButton1, Direction.Up),
			(MouseEventFlags.BUTTON_5_DOWN     , MouseButton.XButton2, Direction.Down),
			(MouseEventFlags.BUTTON_5_UP       , MouseButton.XButton2, Direction.Up),
		};

		private const int WM_INPUT = 0x00FF; //https://docs.microsoft.com/en-us/windows/win32/inputdev/wm-input

		public delegate void RawMouseDown(MouseButton pressed);
		public delegate void RawMouseUp(MouseButton released);
		public delegate void RawMouseScrolled(short dx, short dy);

		public event RawMouseDown? MouseDown;
		public event RawMouseUp? MouseUp;
		public event RawMouseScrolled? MouseScrolled;

		public RawInputHandler(Window messageReceiver) { //pass in the window you want the events to go to
			var targetHwnd = new WindowInteropHelper(messageReceiver).Handle;
			unsafe {
				PInvoke.RegisterRawInputDevices(new Span<RAWINPUTDEVICE>(
					DevicesToRegisterFor.Select(dev => new RAWINPUTDEVICE {
						usUsagePage = dev.usUsagePage,
						usUsage = dev.usUsage,
						dwFlags = RAWINPUTDEVICE_dwFlags.RIDEV_INPUTSINK,
						hwndTarget = (HWND)targetHwnd
					}).ToArray()), (uint)sizeof(RAWINPUTDEVICE));
			}
			HwndSource.FromHwnd(targetHwnd).AddHook(HandleWM_Input);
		}
		~RawInputHandler() => Dispose();
		public void Dispose() {
			unsafe {
				PInvoke.RegisterRawInputDevices(new Span<RAWINPUTDEVICE>(//I don't know if I actually need to do this,
					DevicesToRegisterFor.Select(dev => new RAWINPUTDEVICE {//but I'm slightly scared of Win32
						usUsagePage = dev.usUsagePage,
						usUsage = dev.usUsage,
						dwFlags = RAWINPUTDEVICE_dwFlags.RIDEV_REMOVE
					}).ToArray()), (uint)sizeof(RAWINPUTDEVICE));
			}
		}
		private IntPtr HandleWM_Input(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
			if(msg == WM_INPUT) {
				unsafe {
					uint rawInputCount = 0;
					var rawInputHandle = new HRAWINPUT((nint)lParam.ToInt64());
					PInvoke.GetRawInputData(rawInputHandle, GetRawInputData_uiCommandFlags.RID_INPUT, null, ref rawInputCount, (uint)sizeof(RAWINPUTHEADER));//get count of RAWINPUTs
					var data = new RAWINPUT[rawInputCount];
					fixed(RAWINPUT* dataStart = &data[0]) {
						PInvoke.GetRawInputData(rawInputHandle, GetRawInputData_uiCommandFlags.RID_INPUT, dataStart, ref rawInputCount, (uint)sizeof(RAWINPUTHEADER));//actual data copy
					}

					foreach(var i in data) {
						switch((DeviceType)i.header.dwType) {
							case DeviceType.MOUSE:
								var events = (MouseEventFlags)i.data.mouse.Anonymous.Anonymous.usButtonFlags;
								foreach(var (flag, button, direction) in FlagToButton) {
									if((flag & events) != MouseEventFlags.NONE) {
										switch(direction) {
											case Direction.Up:
												MouseUp?.Invoke(button);
												break;
											case Direction.Down:
												MouseDown?.Invoke(button);
												break;
										}
									}
								}
								if((events & MouseEventFlags.WHEEL) != MouseEventFlags.NONE) {
									MouseScrolled?.Invoke(0, (short)i.data.mouse.Anonymous.Anonymous.usButtonData);
								}
								if((events & MouseEventFlags.HWHEEL) != MouseEventFlags.NONE) {
									MouseScrolled?.Invoke((short)i.data.mouse.Anonymous.Anonymous.usButtonData, 0);
								}
								break;
							case DeviceType.KEYBOARD:
								break;
							case DeviceType.HID:
								break;
						}
					}
				}
				handled = true;
			}
			return IntPtr.Zero;
		}
	}
}