
	; HI-TECH C PRO for the PIC10/12/16 MCU family V9.65PL1
	; Copyright (C) 1984-2009 HI-TECH Software

	; Auto-generated runtime startup code for final link stage.

	;
	; Compiler options:
	;
	; -oPLCBus.cof -mPLCBus.map --summary=default --output=default Main.p1 \
	; --chip=12F510 -P --runtime=default --opt=default -D__DEBUG=1 -g \
	; --asmlist --errformat=Error   [%n] %f; %l.%c %s \
	; --msgformat=Advisory[%n] %s --warnformat=Warning [%n] %f; %l.%c %s
	;


	processor	12F510

	global	_main,start,_exit,reset_vec
	fnroot	_main
	psect	config,class=CONFIG,delta=2
	psect	idloc,class=IDLOC,delta=2
	psect	rbss_0,class=BANK0,space=1
	psect	rbss_1,class=BANK1,space=1
	psect	rbss_2,class=BANK2,space=1
	psect	rbss_3,class=BANK3,space=1
	psect	rdata_0,class=BANK0,space=1
	psect	rdata_1,class=BANK1,space=1
	psect	rdata_2,class=BANK2,space=1
	psect	rdata_3,class=BANK3,space=1
	psect	nvram,class=BANK0,space=1
	psect	nvram_1,class=BANK1,space=1
	psect	nvram_2,class=BANK2,space=1
	psect	nvram_3,class=BANK3,space=1
	psect	nvbit_0,class=BANK0,bit,space=1
	psect	nvbit_1,class=BANK1,bit,space=1
	psect	nvbit_2,class=BANK2,bit,space=1
	psect	nvbit_3,class=BANK3,bit,space=1
	psect	temp,ovrld,class=BANK0,space=1
	psect	struct,ovrld,class=BANK0,space=1
	psect	code,class=CODE,delta=2
	psect	rbit_0,class=BANK0,bit,space=1
	psect	ptbit_0,class=BANK0,bit,space=1
	psect	rbit_1,class=BANK1,bit,space=1
	psect	rbit_2,class=BANK2,bit,space=1
	psect	rbit_3,class=BANK3,bit,space=1
	psect	pstrings,class=CODE,delta=2
	psect	powerup,class=CODE,delta=2
	psect	reset_vec,class=CODE,delta=2
	psect	maintext,class=CODE,delta=2
	C	set	0
	Z	set	2
	PCL	set	2
	INDF	set	0

	psect	fnautoc,class=COMMON,space=1
	psect	common,class=COMMON,space=1
	psect	fnauto0,class=BANK0,space=1
	psect	fnauto1,class=BANK1,space=1
	STATUS	equ	3
	FSR	equ	4
OSCCAL	equ	0x5

	psect	reset_wrap,class=CODE,delta=2
	psect	text,class=CODE,delta=2
	psect	init,class=CODE,delta=2
	psect	init23,class=CODE,delta=2
	psect	end_init,class=ENTRY,delta=2
	psect	clrtext,class=ENTRY,delta=2
	psect	jmp_tab,class=ENTRY,delta=2
	psect	float_text0,class=ENTRY,delta=2,size=512
	psect	float_text1,class=ENTRY,delta=2,size=512
	psect	float_text2,class=ENTRY,delta=2,size=512
	psect	float_text3,class=ENTRY,delta=2,size=512
	psect	float_text4,class=ENTRY,delta=2,size=512
	psect	idata12,class=ENTRY,delta=2
	psect	idata_0,class=CODE,delta=2
	psect	idata_1,class=CODE,delta=2
	psect	idata_2,class=CODE,delta=2
	psect	idata_3,class=CODE,delta=2
	psect	idata_4,class=CODE,delta=2
	psect	idata_5,class=CODE,delta=2
	psect	idata_6,class=CODE,delta=2
	psect	idata_7,class=CODE,delta=2
	psect	rbit_4,class=BANK4,bit,space=1
	psect	rbit_5,class=BANK5,bit,space=1
	psect	rbit_6,class=BANK6,bit,space=1
	psect	rbit_7,class=BANK7,bit,space=1
	psect	rbss_4,class=BANK4,space=1
	psect	rbss_5,class=BANK5,space=1
	psect	rbss_6,class=BANK6,space=1
	psect	rbss_7,class=BANK7,space=1
	psect	rdata_4,class=BANK4,space=1
	psect	rdata_5,class=BANK5,space=1
	psect	rdata_6,class=BANK6,space=1
	psect	rdata_7,class=BANK7,space=1
	psect	nvram_4,class=BANK4,space=1
	psect	nvram_5,class=BANK6,space=1
	psect	nvram_6,class=BANK6,space=1
	psect	nvram_7,class=BANK7,space=1
	psect	nvbit_4,class=BANK4,bit,space=1
	psect	nvbit_5,class=BANK5,bit,space=1
	psect	nvbit_6,class=BANK6,bit,space=1
	psect	nvbit_7,class=BANK7,bit,space=1
	psect	strings12,class=ENTRY,delta=2

	psect	reset_vec,class=CODE,delta=2
reset_vec:
	;oscillator constant would be pre-programmed here
	psect	reset_wrap
reset_wrap:
	movwf	5		;calibrate oscillator
	clrf FSR
	; No powerup routine


	psect	init
start
_exit

;-------------------------------------------------------------------------------
;		Clear (zero) uninitialized global variables

;	2 bytes of RAM objects in bank 0 to zero

	psect	init
	global	__Lrbss_0
	movlw	low(__Lrbss_0)		;load the low address into FSR
	movwf	FSR
	movlw	low(__Lrbss_0+2)		;load the high address+1 into W
	fcall	clear_ram		;go and zero that range of addresses

	psect	clrtext,class=ENTRY,delta=2
	global	clear_ram
;	Called with FSR containing the base address, and
;	W with the last address+1
clear_ram:
	clrwdt			;clear the watchdog before getting into this loop
clrloop:
	clrf	INDF		;clear RAM location pointed to by FSR
	incf	FSR,f		;increment pointer to next location
	xorwf	FSR,w		;XOR with final address
	andlw	1Fh		;Test low bits only
	btfsc	STATUS,2	;have we reached the end yet?
	retlw	0		;all done for this bank, return
	xorwf	FSR,w		;XOR again to restore value
	goto	clrloop		;go and do the next byte

;	No RAM objects to clear in bank 1
;	No RAM objects to clear in bank 2
;	No RAM objects to clear in bank 3
;	2 bytes of RAM objects in common bank to zero

	psect	init
	bsf	FSR,7		;select indirect bank 2/3
	global	__Lcommon
	movlw	low(__Lcommon)		;load the low address into FSR
	movwf	FSR
	movlw	low(__Lcommon+2)		;load the high address+1 into W
	fcall	clear_ram		;go and zero that range of addresses


;-------------------------------------------------------------------------------
	psect	end_init
	clrf	FSR
	ljmp _main
	end	start
