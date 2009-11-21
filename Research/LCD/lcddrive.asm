;****************************************************************
;*   LCDDRIVE.ASM  LCD display drive control	October '96	*
;*   By Marc Simons	msimons@IAEhv.nl			*
;*   Rev. 1+WDT		Only use one HEF4094 + transistor	*
;*   B A S I C   O P E R A T I O N  E X P L A N A T I O N 	*
;****************************************************************
; Pins RB0, RB1 and RB2 are used for controlling AND driving text to the LCD display.
; Most of the time the PIC's are sufficient enough for most applications, exept when
; it comes to more I/O. This simply cannot be expanded, exept when you go to the BIG 
; GUYS like the PIC16C74 etc. where I have done some applications with too.
; Observe the schematics: An PIC16C54 is the heart of the whole thing. It drives the
; HEF4094 CMOS serial2parallel converter. This gives us the databus towards the LCD
; display. Since the HEF4094 strobe is activated at the rising edge, and the LCD
; display on the falling edge, these can be shared. So, on the rising edge the 4094
; spits out it's new byte, and on the falling edge the LCD reads it in. By the way,
; this concept cannot read out info from the LCD display. (Personal opinion: It is
; useless anyway!) Now the hard part comes: How to derive 'text' from 'commands'??
; The LCD has a pin for it: The RS-pin. When it is clear, commands are accepted. when
; set, text is accepted. How is it solved?
;
; Before I spit out a character to the HEF4094, I set the clock for 500uSec. Resistor
; R1 will load capacitor C5. Then, I spit the text character towards the 4094 as soon as
; possible. Therefore the capacitor simply does not have the time to decharge: The LCD
; will accept it as text.
; For commands it is the same, however, of course the other way around: The capacitor 
; must be decharged. T1 forms an emitter follower to buffer the R/C network. The reason
; for this is that the LCD RS input is an TTL input, so without proper buffering it will
; not work.
;
; The following code contains a few basic routines to handle the LCD display. The switch
; that I added is purely for fun: To be able to toggle rotation of the text. I used an 
; 16 characters / 2 lines LCD display from an old security keypad. (Go to a surplus
; electronics store, they always have some!)
;
; P.S. Any suggestions for good code from YOUR side are always welcome!
; Best Regards from msimons@IAEhv.nl, your PIC Scueezer Weezel!
;
LIST	P=16C54 , C=75 , F=INHX8M
;
;  ======= Usage of the ports ======================
;
				;Ra0	not used.
WdtLed		equ	1	;Ra1	wdt-output.
				;Ra2	not used.
Button		equ	3	;Ra3	button input for left/right toggle.
				;
ClockOut	equ	0	;Rb0	clock to HEF4094.
DataOut		equ	1	;Rb1	data to HEF4094.
StrobeOut	equ	2	;Rb2	strobe to HEF4094.
				;Rb3	not used.
				;Rb4	not used.
				;Rb5	not used.
				;Rb6	not used.
				;Rb7	not used.
;
INCLUDE	"c:\mplab\picreg.inc"
;
				;-->Declaration of some variables:
System		equ	08h	; general purpose system control bits.
PointerReg	equ	09h	; character table text pointer register.
wait1		equ	0Ah	; delay variable.
wait2   	equ     0Bh	; delay variable.
OutputReg	equ	0Ch	; output byte to LCD to be shifted out serial.
CounterReg	equ	0Dh	; bit shift counter register.
SpaceCountReg	equ	0Eh	; empty char's to LCD counter register.

Instruct	equ	7	; System bit.
;
;
;**************************************************************************
;*                       Start Main Programm!!!!!!                        *
;**************************************************************************
		org     100H
;
InitMicrochip	call    InitPorts	; initialize ports.
		call	InitLcdDisplay	; startup the LCD as it should be..
		call	StartText	; dump "* marc on air *" on the LCD
		movlw	.4
		call	DumpSpace2Lcd	; dump 4 dummy characters.
		call	YepItWorks	; dump "Yep, It Works!" on the LCD
		movlw	.4
		call	DumpSpace2Lcd	; dump 4 dummy characters.
		call	YepItWorks	; dump "Yep, It Works!" on the LCD
		movlw	.4
		call	DumpSpace2Lcd	; dump 4 dummy characters.
		call	StartText	; dump "* marc on air *" on the LCD
		movlw	.4
		call	DumpSpace2Lcd	; dump 4 dummy characters.
rotateloop1	call	RotateLeftLcd	; initially, rotate text 2 the left.
		clrw
		call	VarWait		; wait a bit and clear watchdog.
		btfsc	porta,Button	; is the button pressed?
		goto	rotateloop1	; no, stay in this loop.
rotateloop2	call	RotateRightLcd	; yep! change LCD to rotate 2 the right.
		clrw
		call	VarWait		; wait a bit and clear watchdog.
		btfsc	porta,Button	; button pressed?
		goto	rotateloop2	; no, stay in this loop.
		goto	rotateloop1
;	
;*********************************
;*   End of main programm	 *
;*********************************
;
		org     0		; routines start at 0h!
;
;*********************************
;*   Init Ports                  *
;*********************************
InitPorts	clrw			; portb all pins are output.
		tris	portb
		clrf	portb		; all outputs low!
		movlw	00fh
		tris	porta		; porta are all inputs.
					;-->Init the Watchdog:
		clrf	rtcc		; RTCC + Prescaler resetten.
		movlw	B'00001111'	; Prescaler op WDT met 1:128. (max)
		option			; rtcc-external without prescaler.
		clrwdt			; reset prescaler and WDT.
		retlw   0
					;
;*********************************
;* Init LCD display routine	 *
;*********************************	;-->Initialize LCD and cursor home:
InitLcdDisplay	bsf	System,Instruct	; indicate that instruction comes.
		clrw			; first send all zero's for LCD startup.
		call	ParallelOut2Lcd	; dump out via 4094, note the Instruct!
		clrw
		call	VarWait		; wait for nice LCD startup! (>2mSec)
		movlw	B'00000001'	; set DB0 bit, display clear instruction.
		call	ParallelOut2Lcd	; dump out via 4094. Instruct = '1'!
		clrw			
		call	VarWait		; wait for clear instruction execution.
		movlw	B'00000011'	; set DB1 bit, return home instruction.
		call	ParallelOut2Lcd	; dump to LCD.
		clrw
		call	VarWait		; wait for home instruction execution.
		movlw	B'00000110'	; Entry Mode Set....
		call	ParallelOut2Lcd	; ....Increment, no Display shift.
		movlw	B'00001110'	; Display on/off control....
		call	ParallelOut2Lcd	; ....On, Cursor, Blink.
		movlw	B'00111000'	; Function set, 2-lines and 8-bits data.
		call	ParallelOut2Lcd
		bcf	System,Instruct	; all instructions done, LCD initialized.
		retlw	0
					;	
;*************************************
;*  "Marc On Air" Text Dump Routine  *
;*************************************	;-->Dump 'Marc On Air' towards LCD: 
StartText	clrf	PointerReg	; kill the text pointer.
nextcharacter	movf	PointerReg,0	; test PointerReg to W.
		call	MarcOnAirText	; get one character from the string.
		andlw	B'11111111'	; skim all bits to test for zero.
		btfsc	status,ze	; are all bits zero?
		retlw	0		; yep!!	End of string!
		call	ParallelOut2Lcd	; dump character to the LCD.
		incf	PointerReg,1	; PointerReg + '1'.
		goto	nextcharacter	; dump next character on the LCD.	
					;-->Here the text string table starts:
MarcOnAirText	addwf	pc,1		; pc = pc + W!
		retlw	2Ah		; write a '*'.
		retlw	0FEh		; blank character.
		retlw	'M'
		retlw	'a'
		retlw	'r'
		retlw	'c'
		retlw	0FEh		; blank character
		retlw	'O'
		retlw	'n'
		retlw	0FEh		; blank character
		retlw	'A'
		retlw	'i'
		retlw	'r'
		retlw	0FEh		; blank character
		retlw	0FEh		; blank character
		retlw	2AH		; write a '*'.
		retlw	0		; will indicate end of string.
					;
;*************************************
;*  "Yep, It Works!!!" Text Routine  *
;*************************************	;-->Dump 'Yep, It Works!!!' towards LCD: 
YepItWorks	clrf	PointerReg	; kill the text pointer.
nextyepchar	movf	PointerReg,0	; test PointerReg to W.
		call	YepText		; get one character from the string.
		andlw	B'11111111'	; skim all bits to test for zero.
		btfsc	status,ze	; are all bits zero?
		retlw	0		; yep!!	End of string!
		call	ParallelOut2Lcd	; dump character to the LCD.
		incf	PointerReg,1	; PointerReg + '1'.
		goto	nextyepchar	; dump next character on the LCD.	
					;-->Here the text string table starts:
YepText		addwf	pc,1		; pc = pc + W!
		retlw	'Y'
		retlw	'e'
		retlw	'p'
		retlw	','
		retlw	0FEh		; blank character.
		retlw	'I'
		retlw	't'
		retlw	0FEh		; blank character.
		retlw	'W'
		retlw	'o'
		retlw	'r'
		retlw	'k'
		retlw	's'
		retlw	021h		; '!'
		retlw	021h		; '!'
		retlw	021h		; '!'
		retlw	0		; will indicate end of string.
;
;*************************************
;*   Rotate Left routine	     *
;*************************************
RotateLeftLcd	bsf	System,Instruct	; indicate to do an instruction.
		movlw	B'00011000'	; shift LCD display left instruction!
		call	ParallelOut2Lcd	; dump out to LCD.
		bcf	System,Instruct	; end of instructions.
		retlw	0
;
;*************************************
;*   Rotate Right routine	     *
;*************************************
RotateRightLcd	bsf	System,Instruct	; indicate to do an instruction.
		movlw	B'00011100'	; shift LCD display right instruction!
		call	ParallelOut2Lcd	; dump out to LCD.
		bcf	System,Instruct	; end of instructions.
		retlw	0
;
;*************************************
;*   Fill empty characters routine   *
;*************************************	;-->Fill LCD with empty characters according to value in W:
DumpSpace2Lcd	movwf	SpaceCountReg	; store number of spaces.
spaceagain	movlw	0FEh		; writa a 'display clear' character....
		call	ParallelOut2Lcd	; ....to the LCD display.
		decfsz	SpaceCountReg	; all spaces done?
		goto	spaceagain	; not yet!
		retlw	0
;
;*************************************
;*   Scalable Delay VarWait	     *
;*************************************
VarWait		movwf	wait2		; store main delay time in wait2.
back		clrwdt			; normally we should not do this in routines....
		decfsz  wait1		; but for this trial it is ok!
        	goto    back
        	decfsz  wait2
        	goto    back
        	retlw   0
;
;*************************************
;*   Mini Delay routine              *
;*************************************
MiniDelay	movlw	.25
		movwf	wait1
tooshort	decfsz  wait1 
       		goto    tooshort
		retlw	0
;
;*************************************
;* Parallel Output Routine to LCD    *
;*************************************	;-->Duration routine approx. 500uSec.
ParallelOut2Lcd	movwf	OutputReg	; store byte to send!
		bcf	portb,ClockOut	; clock low to start with.
		btfss	System,Instruct	; instruction or regular character?
		bsf	portb,ClockOut	; set clock to load capacitor.
		movlw	.140
		movwf	wait1
loadcaploop	decfsz  wait1 		; (good for LCD execution time too!)
		goto    loadcaploop	; load RS capacitor when clock set!
		bsf	portb,ClockOut	; clock up to 4094 period.
		movlw   .8		; 8 bits to go.
		movwf   CounterReg
nextout		bcf     portb,ClockOut	; lower the clock.
		bcf     portb,DataOut	; initially, data output low
		rlf	OutputReg,1	; rotate OutputReg through carry.
		btfsc	status,cy	; check out if carry is '1'.
		bsf	portb,DataOut	; yep! so set the DataOut.
		bsf	portb,ClockOut	; clock into 4094.
		decfsz	CounterReg	; 8 bits done?
		goto	nextout		; no, do some more bits.
		bcf	portb,ClockOut	; clock low.
		bsf	portb,StrobeOut	; create nice strobe to 4049.
		bcf	portb,StrobeOut	; strobe low.
		retlw   0
;
;****************************************
;*  End of routines...............      *
;****************************************
;
		org	1FFh		; in case of PIC16C54
		goto	InitMicrochip
		org	3FFh		; in case of PIC16C56
		goto	InitMicrochip
					;
					;
		end

