#ifndef I2C_H
#define I2C_H

#include "Default.h"

#define SDA_TRIS TRISCbits.TRISC3
#define SDA PORTCbits.RC3
//#define SDA LATCbits.LATC3
#define SDATRISBYTE _TRISC
#define SDABYTE _PORTC
#define SDABIT	3

#define SCL_TRIS TRISCbits.TRISC4
#define SCL PORTCbits.RC4
#define SCLTRISBYTE _TRISC
#define SCLBYTE _PORTC
#define SCLBIT	4

void I2CInit();
void I2CStart();
void I2CStop();
void I2CWrite(int8 _Byte);
int8 I2CRead();
int8 I2CAck();

void I2CStartSlow();
void I2CStopSlow();
void I2CWriteSlow(int8 _Byte);
int8 I2CReadSlow();
int8 I2CAckSlow();
void I2CDelaySlow();

#endif
