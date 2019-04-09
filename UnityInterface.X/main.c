/* 
 * File:   main.c
 * Author: Tom Birdsong
 * Purpose: Direct mcu operations for Unity <-> peripheral communications
 *
 * Last updated: April 2, 2019
 */

#include <plib.h>
#include <proc/p32mx150f128d.h>

/* PIN DIAGRAM
 * 5    - U1RX
 * 18   - Analog0 (** NOT 5V TOLERANT **)
 * 19   - Analog1 (** NOT 5V TOLERANT **)
 * 20   - Button2 (joystick button)
 * 22   - LED
 * 23   - U1TX
 * 37   - INT0
 * 40   - GND
 * 
 * Also using TMR3 for analog read timings
 */

void setupPins();
void setupInterrupts();
void setupADC();
void setupUART();
void makeMessage(char*,char,char,int*,char*);
void sendMessage(char*,int);

void OnINT0();
void OnINT1();
void OnTMR3();
void OnRXDone();
void OnTXReady();
void OnADCReady();
void printbyte(char);

const int TIMER_DELAY = 1000;
const int NUM_SAMP_TO_IF = 4;

const int BAUD_RATE = 9600;
const int MAX_REC_BUF_LEN = 8;
unsigned char buttonEdgeDir = 1; //default to rising edge detection

char recbuf[8] = {0,0,0,0,0,0,0,0};
char recbuflen = 0;             //default to empty buffer
int bufReady = 0;               //flag controlled by SW

int joystick_x = 0;
int joystick_y = 0;
float temp = 0;



main() {
    
    setupPins();
    setupInterrupts();
    setupADC();
    setupUART();
    
    //Test transmission
    char test[5] = {'a','w','a','k','e'};
    sendMessage(test,5);
     
    while(1) {
        
        //sendMessage(out,*count);
        
        if(bufReady) {
            PORTBbits.RB2 = 1;
            recbuflen = 0;
            bufReady = 0;
        }
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
}

void setupInterrupts() {
    
   INTEnableSystemMultiVectoredInt();  //handle interrupts
    
    //INT0 and INT1
    TRISBbits.TRISB7 = 1;
    TRISBbits.TRISB0 = 1;
    
    INTCONbits.INT0EP = buttonEdgeDir;  //rising edge
    IEC0bits.INT0IE = 1;    //enable
    IPC0bits.INT0IP = 1;    //priority 1
    
    //joystick button
    PPSInput(4,INT1,RPB0);
    INTCONbits.INT1EP = buttonEdgeDir;
    IPC1bits.INT1IP = 1;
    IPC1bits.INT1IS = 1;
    IEC0bits.INT1IE = 1;
    
    //U1RX
    IFS1bits.U1RXIF = 0;
    IPC8bits.U1IP = 2;
    IPC8bits.U1IS = 1;
    IEC1bits.U1RXIE = 1;
    
    //AD1 for channels A0 and A1
    IEC0bits.AD1IE = 1;
    IPC5bits.AD1IP = 1;
    IPC5bits.AD1IS = 1;
    IFS0bits.AD1IF = 0;
    
    //TMR2
    IEC0bits.T2IE = 1;
    IPC2bits.T2IP = 1;
    IPC2bits.T2IS = 1;
    
}

void setupADC() {
    //TODO: configure to use A0 and A1, not auto-sample
    
    //Set up timer
    T3CONbits.TGATE = 0;
    T3CONbits.TCS = 0;
    T3CONbits.TCKPS = 3;
    PR3 = TIMER_DELAY;
    
    //configure ADC
    //table 17.4
    AD1CHSbits.CH0SB = 0; //select scan
    //AD1CHSbits.CH0SA = 0; //select A0
    AD1CON1bits.FORM = 0; //use 16-bit integer output
    AD1CON1bits.SSRC = 2; //TMR3 period determines conversion start
    
    //scan channels A0 and A1
    AD1CSSL = 0x0003;
    
    AD1CON2bits.VCFG = 0; //use built-in VR+ and VR-
    AD1CON2bits.CSCNA = 1; //Do scan
    AD1CON2bits.SMPI = NUM_SAMP_TO_IF - 1; //interrupt at completion of 4 conversions
    AD1CON2bits.BUFM = 0; //buffer configured as one 16-word buffer
    
    AD1CON2bits.ALTS = 0; //always use MUX A (don't alternate))
    AD1CON3bits.SAMC = 8; //auto sample at 8 TADs
    AD1CON3bits.ADRC = 0; //ADC conversion clock derived from PBCLK
    AD1CON3bits.ADCS = 2; //ADC conversion clock: 8*TPB = TAD
    
    AD1CON1bits.ASAM = 1; //Sample auto-start
    
    AD1CON1bits.ON = 1;
    T3CONbits.ON = 1;
    
}

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
}

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

//Adds terminating characters '\n' and '\r'
void sendMessage(char* message, int len) {
    const char NL = '\n';
    const char CR = '\r';
    int i = 0;
    char send_val = 0;
    
    for(; i < len+2; i++) {
        if(i < len) {
            send_val = *(message+i);
        } else if(i == len) {
            send_val = CR;
        } else {
            send_val = NL;
        }
        U1TXREG = send_val;
        while(U1STAbits.UTXBF == 1);
        IFS1bits.U1TXIF = 0;
    }
        
}

void __ISR(3) OnINT0() {
    
    char val = INTCONbits.INT0EP + '0';
    char out[3] = {'b','1',val};
    sendMessage(out,3);

    INTCONbits.INT0EP = (INTCONbits.INT0EP == 0) ? 1 : 0;  //toggle edge
    
    IFS0bits.INT0IF = 0;   
}

void __ISR(7) OnINT1() {
    char val = INTCONbits.INT1EP + '0';
    char out[3] = {'b','2',val};
    sendMessage(out,3);
    
    //INTCONbits.INT1EP = (INTCONbits.INT1EP == 0) ? 1 : 0; 
    
    IFS0bits.INT1IF = 0;
}

void __ISR(32) OnRXDone() {
    char rx_byte = 0;
    if((rx_byte = U1RXREG) != 0) {
                
        if(recbuflen >= MAX_REC_BUF_LEN) {
            bufReady = 1;
        } else {
            recbuf[recbuflen] = rx_byte;
            recbuflen++;
        }
        
        bufReady = (recbuflen >= MAX_REC_BUF_LEN) ? 1 : 0;
        
        //test receive
            char params[1];
        params[0] = recbuflen+65;
        int count[1] = {3};
        char sel1 = 'r';
        char sel2 = 'x';
        char out[5];
        makeMessage(out,sel1,sel2,count,params);
        sendMessage(out,*count);
    }
                        
    IFS1bits.U1RXIF = 0;  
}
void OnTXReady();

void __ISR(23) OnADCReady() {
    unsigned int samples_x[2] = {0,0};
    unsigned int samples_y[2] = {0,0};

    samples_x[0] = ADC1BUF0;
    samples_y[0] = ADC1BUF1;
    samples_x[1] = ADC1BUF2;
    samples_y[1] = ADC1BUF3;
    
    int mean_x = 0, mean_y = 0, i = 0;
    for( ; i< NUM_SAMP_TO_IF / 2; i++) {
        mean_x += (samples_x[i] >>3); //FIXME
    }
    mean_x /= (NUM_SAMP_TO_IF / 2);
    
    for( i = 0; i< NUM_SAMP_TO_IF / 2; i++) {
        mean_y += (samples_y[i] >>3);   //FIXME
    }
    mean_y /= (NUM_SAMP_TO_IF / 2);
        
    //output if delta
    if(joystick_x != mean_x || joystick_y != mean_y) {
        const int OUT_LEN = 4;
        char out[4] = {'j','1',(int)mean_x,(int)mean_y};
        sendMessage(out,OUT_LEN);
    }
    
    joystick_x = mean_x;
    joystick_y = mean_y;
    T3CONbits.ON = 1;
    
    IFS0bits.AD1IF = 0;
    
}

void printbyte(char b) {
    LATC = b;
    LATBbits.LATB0 = (b & 0x40)>>6;
};