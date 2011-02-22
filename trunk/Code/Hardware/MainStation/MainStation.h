#ifndef MAINSTATION_H
#define MAINSTATION_H

#include "Default.h"

#define LED_LATCH LATBbits.LATB4
#define LED_TRIS TRISBbits.TRISB4
#define LED LATBbits.LATB4

void MSInit();
void MSUpdate();

void SetLED(int8 _State);

int8 GetLED();

#endif