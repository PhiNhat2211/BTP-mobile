#pragma once

enum
{
     VMT_TU_SoftwareInfo	                      = 1,
     VMT_TU_ConnectionStatus		              = 2,
     VMT_TU_MachineNotice		                  = 3,
     VMT_TU_SetupDone	     	                  = 4, //new  nopayload
     //---------------------
     VMT_TU_GetUserAccessRole	              = 21,
     VMT_TU_SetLogin4Machine	              = 22,
     VMT_TU_SendMachineStatusChange	  = 23,
     //---------------------- common Sensing
     VMT_TU_ResetDGPS	                          = 40,
     VMT_TU_SendPubx05	                      = 41,
     VMT_TU_SendPubx06	                      = 42,
     VMT_TU_SendHighForward	              = 43,
     VMT_TU_SendHighBackward	              = 44,
     VMT_TU_RequestCfgekf	                      = 45,
     VMT_TU_SaveDGPSCfg		                  = 46,
     //----------------------- Availiable
     VMT_TU_MaChineStopCodeList		      = 60,
     VMT_TU_GetMachineStop		              = 61,
     VMT_TU_SetMachineStop		              = 62,
     VMT_TU_NotifyMachineStopResult        = 63, 
     //---------------------- Alarm
     VMT_TU_NotifyAlarm                           = 65,
     //------------------------ Common Job
     VMT_TU_JobOrder		                      = 80,
     VMT_TU_JobCancel		                      = 81,
     VMT_TU_JobDone		                          = 82,
     VMT_TU_ManualJobDone		              = 83,
     VMT_TU_JobCancelAll		                  = 84,
     //------------------------ ITV Only
     VMT_TU_ITV_DGPS_Periodic		          = 100,
     VMT_TU_ITV_PDS		                          = 101,
     //-----
     VMT_TU_ITV_SetChassis_Attach		      = 110,
     VMT_TU_ITV_NotifyChassis_Attach		  = 118,//new 
     //-----
     VMT_TU_ITV_NofityBlockEnterance		  = 111,
     VMT_TU_ITV_NotifyCPSAlign		          = 112,
     VMT_TU_ITV_NotifyPOW		              = 113,
     VMT_TU_ITV_SetManualReady		      = 114,
     VMT_TU_ITV_NotifyManualReady		  = 115,
     VMT_TU_ITV_SetManualArrival		      = 116,
     VMT_TU_ITV_NotifyManualArrival		  = 117,
     //------------------------ RTG Only
     VMT_TU_RTG_PDS_Periodic		          = 200,
     VMT_TU_RTG_CPS_Align		                  = 201,
     VMT_TU_RTG_PDS_PickDrop 	              = 202,
     VMT_TU_RTG_RFID 		                      = 203,
     VMT_TU_RTG_PDS_PickDropConfirm       = 204,
         //**** ITV
     VMT_TU_RTG_NotifyMachinePOW		  = 220,
     VMT_TU_RTG_NotifyMachineBlockEnter  = 222,
     VMT_TU_RTG_NotifyMachineReadyITV   = 223,
     VMT_TU_RTG_ManualReady		              = 224,
     VMT_TU_RTG_NotifyManualReady		      = 225,
         //**** Inven
     VMT_TU_RTG_SendBlockInfo	              = 240,
     VMT_TU_RTG_NotifyBlockInfo	              = 241,
     VMT_TU_RTG_SendBlockInfoSimple	      = 242,
     VMT_TU_RTG_NotifyBlockInfoSimple	      = 243,
     VMT_TU_RTG_SendCorrection	              = 244,
     VMT_TU_RTG_NotifyCorrection	          = 245,
        //**** Job
     VMT_TU_RTG_SetCurrentJob		          = 260,
     VMT_TU_RTG_NotifySetCurrentJob		  = 263,
     VMT_TU_RTG_HandleJobDone		          = 261,
     VMT_TU_RTG_TargetJob		              = 262,
       //----- Marring
     VMT_TU_RTG_Marring                        = 270,     
     VMT_TU_RTG_Swap_Result                = 271,
     VMT_TU_RTG_Return_Cntr                 = 272,
     VMT_TU_RTG_NotifyMarring                = 273, 

     //------------------------ EH RS Only
     VMT_TU_RSEH_PDS_Periodic                    = 300,
     VMT_TU_RSEH_PDS_PickDrop                   = 301,
     VMT_TU_RSEH_PDS_RFID                        = 302,
     VMT_TU_RSEH_PDS_PickDropConfirm        = 303, //new
     //**** EH RS
     VMT_TU_RSEH_NotifyMachinePOW		      = 320,
     VMT_TU_RSEH_NotifyMachineBlockEnter    = 322,
     VMT_TU_RSEH_NotifyMachineReadyITV     = 323,
     VMT_TU_RSEH_ManualReady		              = 324,
     VMT_TU_RSEH_NotifyManualReady		      = 325,
     //**** Inven
     VMT_TU_RSEH_SendBlockInfo	                  = 340,
     VMT_TU_RSEH_NotifyBlockInfo	              = 341,
     VMT_TU_RSEH_SendBlockInfoSimple	      = 342,
     VMT_TU_RSEH_NotifyBlockInfoSimple	      = 343,
     VMT_TU_RSEH_SendCorrection	              = 344,
     VMT_TU_RSEH_NotifyCorrection	              = 345,
     //**** Job
     VMT_TU_RSEH_SetCurrentJob		              = 360,
     VMT_TU_RSEH_NotifySetCurrentJob		      = 363,
     VMT_TU_RSEH_HandleJobDone		          = 361,
     VMT_TU_RSEH_TargetJob		                  = 362,
     //----- Marring
     VMT_TU_RSEH_Marring                            = 370,     
     VMT_TU_RSEH_Swap_Result                    = 371,
     VMT_TU_RSEH_Return_Cntr                     = 372,
     VMT_TU_RSEH_NotifyMarring                    = 373 
};
