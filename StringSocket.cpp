/*
 * This class servers as an equivalent to string socket class on the 
 * spreadsheet client
 * Scott Young
 * Dharani Adhikari
 * Mathnew Greave
 * Jonathan Warner
 */
#include <iostream>
#include <string>
#include "StringSocket.h"

using namespace std;

namespace FinalProject
{
  
  class StringSocket
  {


    // constructor
    StringSocket::StringSocket(socket s, Encoding e)
    {
    }

    /*
     * This method sends a message through the socket to clients
     */
    void beginSend(string message, CallbackType callback, int clientID)
    {
    }

    /*
     * This method receives message from clients
     */
    void beginReceive(CallbackType callback, int clientID)
    {
    }

    /*
     * This method is to safely close the socket connectio 
     */
    void close()
    {
      
    }
  };

}//end namespace
#endif
