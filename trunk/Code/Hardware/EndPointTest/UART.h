#ifndef UART_H
#define UART_H

#include "Default.h"

#define UARTClearErrors\
{\
if (OERR){TXEN=0;TXEN=1;CREN=0;CREN=1;}\
if (FERR){TXEN=0;TXEN=1;}\
}

static int8 UARTBuffer[16];
static int8 UARTBufferSize;

#define UART_DIR RB6
#define UART_DIR_TRIS TRISB6

extern void UARTWrite();
extern void UARTRead();
extern void UARTInit();
extern int8 UARTReadInt8(void);
extern int16 UARTReadInt16();
extern int8 UARTReadBool(void);
extern int8 UARTAvailable(void);
extern void UARTWriteInt8(int8 c);
extern void UARTWriteInt16(int16 _Value);
#endif
