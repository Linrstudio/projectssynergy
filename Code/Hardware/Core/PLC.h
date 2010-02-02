#include <pic18.h>
#include "Default.h"

void PLCInit()
{
	//our PLC needs a high frequenty signal, say 1Mhz ?

	TMR0ON=1;//enable timer
	T08BIT=1;//use 8bit devider
	T0CS=0;//use internal clock
	T0CE=0;//choose edge ( doesnt really matter )
	PSA=0;
	T0PS0=0;
	T0PS1=0;
	T0PS2=1;
}
