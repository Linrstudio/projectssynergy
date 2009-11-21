;ษออออออออออออออออออออออออออออออออออออออป
;บ PICREG.INC				บ
;ศออออออออออออออออออออออออออออออออออออออผ
;
PIC54   equ     1FFH            ; Define Reset Vectors
PIC55   equ     1FFH
PIC56   equ     3FFH
PIC57   equ     7FFH
PIC71   equ     000H
vector	equ	004h	; 16C71 Interrupt vector address.
			;-->SFR registers for the PIC16CXX series:
f0	equ	00h	; indirect file register.
rtcc    equ     01h     ; rtcc.
pc      equ     02h     ; programm counter.
pcl     equ     02h     ; programm counter low byte.
status  equ     03h     ; status register.
fsr     equ     04h     ; file select register.
porta   equ     05h     ; porta I/O.
portb   equ     06h     ; portb I/O.
portc   equ     07h     ; portc I/O.
;
adres	equ     09h     ; 16C71 A/D result register.
adcon0	equ     08h	; 16C71 A/D control and status register.
adcon1	equ     08h     ; 16C71 A/D control register.
pclath	equ     0ah     ; 16C71 Programm counter 5 MSBits.
intcon	equ     0bh     ; 16C71 Interrupt control register.
trisa	equ     05h     ; 16C71 PortA I/O control register.
trisb	equ     06h     ; 16C71 PortB I/O control register.
opt	equ	01h	; 16C71 Option register.
			;--> STATUS REG. Bits
carry	equ     0h      ; Carry Bit.
cy	equ     0h
dcarry	equ     1h	; Decimal carry bit.
dc	equ     1h
z_bit	equ     2h      ; Zero bit.
ze	equ     2h
p_down	equ     3h	; Power down bit.
pd	equ     3h
t_out	equ     4h	; Timeout bit.
to	equ     4h
pa0	equ     5h      ;16C5X Page address0 selection bit.
pa1	equ     6h      ;16C5X Page address1 selection bit.
pa2	equ     7h      ;16C5X Page address2 selection bit.
rp0	equ     5h      ;16C71 Register page select bit0.
rp1	equ     6h      ;16C71 Register page select bit1.
irp	equ     7h      ;16C71 Register page select bit for indirect addr.
			;-->Interrupt register bits:
gie	equ     7h      ;16C71 INTCON global interrupt enable bit.
adie	equ     6h      ;16C71 INTCON A/D interrupt enable bit.
rtie	equ     5h      ;16C71 INTCON RTCC overflow interrupt enable bit.
inte	equ     4h      ;16C71 INTCON INTF interrupt enable bit.
rbie	equ     3h      ;16C71 INTCON RBIF interrupt enable bit.
rtif	equ     2h      ;16C71 INTCON RTCC overflow interrupt flag.
intf	equ     1h      ;16C71 INTCON Interrupt flag on INT input portb,0.
rbif	equ     0h      ;16C71 INTCON Interrupt flag on portb,4..7 change.
			;-->A/D conversion control bits:
adcs1	equ     7h      ;16C71 ADCON0 A/D conversion clock select1.
adcs0	equ     6h      ;16C71 ADCON0 A/D conversion clock select0.
chs1	equ     4h      ;16C71 ADCON0 Analog channel select bit1.
chs0	equ     3h      ;16C71 ADCON0 Analog channel select bit0.
go	equ     2h      ;16C71 ADCON0 A/D conversion start bit.
adif	equ     1h      ;16C71 ADCON0 A/D complete interrupt flag.
adon	equ     0h      ;16C71 ADCON0 A/D on/off flag.
			;-->A/D conversion control register bits:
pcfg1   equ     1h      ;16C71 ADCON1 port config bit for A/D converter.
pcfg0   equ     0h      ;16C71 ADCON1 port config bit for A/D converter.
			;-->Boolean bits:
same    equ     1h
lsb     equ     0h
msb     equ     7h
true    equ     1h
yes     equ     1h
false   equ     0h
no	equ     0h
Err	equ	7h
err	equ	7h
;
;
;              *** end ***
