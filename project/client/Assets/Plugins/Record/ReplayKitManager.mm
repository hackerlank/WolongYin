//
//  ReplayKitManager.mm
//
//  Created by Tzt on 15/12/11.
//  Copyright © 2015年 Tzt. All rights reserved.
//

#import "ReplayKitProxy.h"


extern "C"
{
    bool isSupportRecord()
    {
       return [ReplayKitProxy isSupportReplay] == YES;
    }
    
    void startRecording()
    {
        [[ReplayKitProxy sharedInstance] startRecording];
    }
    
    void stopRecording()
    {
        [[ReplayKitProxy sharedInstance] stopRecording];
    }
    
    void discardRecording()
    {
        [[ReplayKitProxy sharedInstance] discardRecording];
    }
    
    void displayRecordingContent()
    {
        [[ReplayKitProxy sharedInstance] displayRecordingContent];
    }
    
}