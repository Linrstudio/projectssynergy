#ifndef UART_H
#define UART_H

#include "Default.h"

#define UART_TX_TRIS	TRISBbits.TRISB7
#define UART_RX_TRIS	TRISBbits.TRISB5
#define UART_DIR_TRIS	TRISBbits.TRISB6

#define UART_TX			PORTBbits.RB7
#define UART_RX			PORTBbits.RB5
#define UART_DIR		PORTBbits.RB6

#define UARTClearErrors \
{\
if (RCSTAbits.OERR){TXSTAbits.TXEN=0;TXSTAbits.TXEN=1;RCSTAbits.CREN=0;RCSTAbits.CREN=1;}\
if (RCSTAbits.FERR){TXSTAbits.TXEN=0;TXSTAbits.TXEN=1;}\
}

static int8 UARTError=0;

extern void UARTWrite();
extern void UARTRead();
extern void UARTInit();
extern int8 UARTReadInt8();
extern int16 UARTReadInt16();
extern int8 UARTAvailable();
extern void UARTWriteInt8(int8 c);
extern void UARTWriteString(register const char *str);
extern void UARTWriteInt16(int16 _Value);
#endif