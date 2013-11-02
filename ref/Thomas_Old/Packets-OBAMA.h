
class Packets
{
    public:
    //typedef unsigned char BYTE;
    //typedef unsigned short USHORT;
    //typedef unsigned int WORD;
    //typedef unsigned long DWORD;

    enum EPACKET_TYPE_ID
    {
         AUTHLOGINPACKET = 0x41b,
         AUTHLOGINRESPONSE = 0x41f

    };

    struct SHEADER
    {
	unsigned int uiSize;
	EPACKET_TYPE_ID Type;
    };

    struct SAuthLoginPacket
    {
	SHEADER Head;
	char szUsername[16];
	unsigned long ulPassword[4];
	char szServer[16];
    };

    struct SAuthLoginReplyPacket
    {
	SHEADER Head;
	unsigned long ulKey2;
	unsigned long ulKey1;
	char szIPAddress[16];
	unsigned long ulPort;

	void AuthLoginReplyPacket()
	{
        Head.uiSize = sizeof(SAuthLoginReplyPacket);
        Head.Type = AUTHLOGINRESPONSE;
	}
    };






        Packets();
        ~Packets();
    protected:
    private:
};
