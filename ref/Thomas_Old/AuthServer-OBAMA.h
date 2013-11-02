#include <winsock.h>

class AuthServer
{
    public:
        AuthServer();  //ctor
        char * IP;
        unsigned char port;
        ~AuthServer(); //dtor
    protected:
    private:
    SOCKET sListen;
    SOCKET sConnect;
    SOCKET sAccept;
    char inbuf[1];
};
