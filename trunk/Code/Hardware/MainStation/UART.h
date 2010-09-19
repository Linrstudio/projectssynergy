#ifndef UART_H
#define UART_H

#include "Default.h"

#define UART_TX_TRIS	TRISBbits.TRISB7
#define UART_RX_TRIS	TRISBbits.TRISB5
#define UART_DIR_TRIS	TRISBbits.TRISB6

#define UART_TX			PORTBbits.RB7
#define UART_RX			PORTBbits.RB5
//#define UART_DIR		LATBbits.LATB6
#define UART_DIR		PORTBbits.RB6

#define UARTClearErrors \
{\
if (RCSTAbits.OERR){TXSTAbits.TXEN=0;TXSTAbits.TXEN=1;RCSTAbits.CREN=0;RCSTAbits.CREN=1;}\
if (RCSTAbits.FERR){TXSTAbits.TXEN=0;TXSTAbits.TXEN=1;}\
}

static int8 UARTError=0;

void UARTWrite();
void UARTRead();
void UARTInit();
int8 UARTReadInt8();
int16 UARTReadInt16();
int8 UARTAvailable();
void UARTWriteInt8(int8 c); 
void UARTWriteInt16(int16 _Value);
#endif