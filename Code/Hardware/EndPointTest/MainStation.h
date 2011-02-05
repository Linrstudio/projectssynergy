#ifndef MAINSTATION_H
#define MAINSTATION_H

#include "Default.h"


#define LED1A_LATCH LATCbits.LATC0
#define LED1B_LATCH LATCbits.LATC1

#define LED2A_LATCH LATCbits.LATC2
#define LED2B_LATCH LATBbits.LATB4

#define LED1A_TRIS TRISCbits.TRISC0
#define LED1B_TRIS TRISCbits.TRISC1

#define LED2A_TRIS TRISCbits.TRISC2
#define LED2B_TRIS TRISBbits.TRISB4

#define LED1A LATCbits.LATC0
#define LED1B LATCbits.LATC1
			  
#define LED2A LATCbits.LATC2
#define LED2B LATBbits.LATB4

void MSInit();
void MSUpdate();

void SetLED1(int8 _State);
//void SetLED2(int8 _State);

int8 GetLED1();
int8 GetLED2();

#endif