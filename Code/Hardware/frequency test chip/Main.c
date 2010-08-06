#include <pic.h>

// 120Khz @ 6Mhz crystal

void main()
{
	GPWUF=0;
	//CWUF=0;

	OPTION=PS2|PS0;
	ANS0=0;			//disable analog
	ANS1=0;			//disable analog
	TRIS=0b101011;
LOOP:
		GP2=1;
		asm("nop");
		asm("nop");

		asm("nop");
		asm("nop");
		asm("nop");


		GP2=0;
		asm("nop");
		asm("nop");

		asm("nop");
		asm("nop");
		asm("nop");


		GP2=1;
		asm("nop");
		asm("nop");

		asm("nop");
		asm("nop");
		asm("nop");


		GP2=0;
		asm("nop");
		asm("nop");
		asm("nop");
		asm("nop");
goto LOOP;
}
