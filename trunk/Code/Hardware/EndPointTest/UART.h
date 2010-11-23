#ifndef UART_H
#define UART_H

#include "Default.h"

#define UART_TX_TRIS	TRISB7
#define UART_RX_TRIS	TRISB5
#define UART_DIR_TRIS	TRISB6

#define UART_TX			RB7
#define UART_RX			RB5
#define UART_DIR		RB6

#define UARTClearErrors \
{\
if (OERR){TXEN=0;TXEN=1;CREN=0;CREN=1;}\
if (FERR){TXEN=0;TXEN=1;}\
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