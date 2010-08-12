#ifndef UART_H
#define UART_H

#include "Default.h"

#define UARTClearErrors \
{\
if (RCSTAbits.OERR){TXSTAbits.TXEN=0;TXSTAbits.TXEN=1;RCSTAbits.CREN=0;RCSTAbits.CREN=1;}\
if (RCSTAbits.FERR){TXSTAbits.TXEN=0;TXSTAbits.TXEN=1;}\
}

extern void UARTWrite();
extern void UARTRead();
extern void UARTInit();
extern int8 UARTReadInt8(void);
extern int16 UARTReadInt16();
extern int8 UARTAvailable(void);
extern void UARTWriteInt8(int8 c);
extern void UARTWriteString(register const char *str);
extern void UARTWriteInt16(int16 _Value);
#endif