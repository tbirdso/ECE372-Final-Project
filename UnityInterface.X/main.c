/* 
 * File:   main.c
 * Author: Tom Birdsong
 * Purpose: Direct mcu operations for Unity <-> peripheral communications
 *
 * Last updated: April 2, 2019
 */

#include <plib.h>

void setupPins();
void setupUART();
void makeMessage(char*,char,char,int*,char*);
void sendMessage(char*,int);

void OnINT0();
void OnTimer();
void OnRXDone();
void OnTXReady();
void OnADCReady();
void printbyte(char);

const int BAUD_RATE = 9600;

int joystick_x = 0;
int joystick_y = 0;
float temp = 0;


main() {
    
    setupPins();
    setupUART();
    
    char params[2] = {'a','b'};
    int count[1] = {2};
    char sel1 = '\n';
    char sel2 = '0';
    char out[4] = {0,0,0};
    makeMessage(out,sel1,sel2,count,params);
    sendMessage(out,*count);
    
    while(1) {
        
        sendMessage(out,*count);
    }
    
}

void setupPins() {

    TRISB = 0;
    TRISC = 0;
    ANSELB = 0;
    ANSELC = 0;
    LATB = 0;
    LATC = 0;
    
    TRISCbits.TRISC6 = 1;   // pin 5 - U1TX
    
    PPSOutput(1, RPB3, U1TX);
    PPSInput(3, U1RX, RPC6);
};

void setupUART() {
    OSCCONbits.PBDIV = 3;   //PBCLK is SYSCLK divided by 8
    U1STA = 0;        // clear status
    U1MODE = 0;    // clear mode
    
    U1BRG = 25;         //Expected: 9600, Observed: 1200
    
    U1STA = 0x1800;     //Enable RX and TX
    
    U1MODE = 0x8000;    // Enable UART for 8-bit data
                        // No Parity, 1 Stop bit
                        // (source: PIC32 Datasheet - 21)
    
    U1STASET = 0x1400;  // Enable Transmit and Receive
};

void makeMessage(char* out, char s1,char s2,int* len, char* params) {
    int i = 0;
    int count = *len;
    
    *out = s1;
    *(out+1) = s2;
    
    for(; i < count; i++) {
        *(out+2+i) = (*(params + i));
    }
    
    *len = count+2;
    //NL and CR to be appended when sent
};

//Inputs MUST have \n and \r at tail!
void sendMessage(char* message, int len) {
    const char NL = '\n';
    const char CR = '\r';
    int i = 0;
    char send_val = 0;
    
    for(; i < len+2; i++) {
        if(i < len) {
            send_val = *(message+i);
        } else if(i == len) {
            send_val = '\n';
        } else {
            send_val = '\r';
        }
        U1TXREG = send_val;
        while(U1STAbits.UTXBF == 1);
        IFS1bits.U1TXIF = 0;
    }
        
}

void OnINT0();
void OnTimer();
void OnRXDone();
void OnTXReady();
void OnADCReady();

void printbyte(char b) {
    LATC = b;
    LATBbits.LATB0 = (b & 0x40)>>6;
};