//
//  ReplayKitProxy
//
//  Created by Tzt on 15/12/11.
//  Copyright © 2015年 Tzt. All rights reserved.
//

#import "ReplayKitProxy.h"

@interface  ReplayKitProxy () <RPPreviewViewControllerDelegate, RPScreenRecorderDelegate>
@property RPPreviewViewController* previewViewController;
@end


@implementation ReplayKitProxy

+ (ReplayKitProxy *)sharedInstance
{
    static ReplayKitProxy* _instance = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        _instance = [[ReplayKitProxy alloc] init];
    });
    return _instance;
}

- (BOOL)isSupportReplay
{
    NSString* version = [[UIDevice currentDevice] systemVersion];
    
    BOOL _ios90orNewer = [version compare: @"9.0" options: NSNumericSearch] != NSOrderedAscending;
    
    return _ios90orNewer;
}

// 开始录制视频
- (void)startRecording
{
    RPScreenRecorder* recorder = RPScreenRecorder.sharedRecorder;
    
	if([ReplayKitProxy isSupportReplay] == NO){
		NSLog(@"ReplayKitProxy isSupportReplay iso>9.0");
		return;
	}

    if (recorder.available == FALSE) {
        NSLog(@"Replaykit is not available");
        return;
    }
    
    if (recorder.recording == TRUE) {
        NSLog(@"Replaykit is recording");
        return;
    }
    
    // 添加代理:防止有些设备录制出来黑屏
    recorder.delegate = self;
    
    [recorder startRecordingWithMicrophoneEnabled:YES handler:^(NSError * _Nullable error) {
        if (error) {
            NSLog(@"%@", error.localizedDescription);
        }
    }];
}

// 停止录制视频
- (void)stopRecording
{
    RPScreenRecorder* recorder = RPScreenRecorder.sharedRecorder;
    if (recorder.recording == FALSE) {
        return;
    }
    
    [recorder stopRecordingWithHandler:^(RPPreviewViewController * _Nullable previewViewController, NSError * _Nullable error) {
        if (error) {
            NSLog(@"%@", error.localizedDescription);
            // [self ShowRecordAlert:error.localizedDescription];
            return;
        } else {
            if (previewViewController != NULL) {
                previewViewController.previewControllerDelegate = self;
                self.previewViewController = previewViewController;
            }
        }
    }];
}

// 删除已录制视频，必须在stopRecording之后调用
// eg.离开视频分享界面
- (void)discardRecording
{
    RPScreenRecorder* recorder = RPScreenRecorder.sharedRecorder;
    if (recorder.recording == TRUE) {
        return;
    }
    
    [recorder discardRecordingWithHandler:^{
        NSLog(@"discardRecording complete");
        self.previewViewController = NULL;
    }];
}

// 显示视频
- (void)displayRecordingContent
{
    UIViewController* rootViewController = [UIApplication sharedApplication].keyWindow.rootViewController;
    [rootViewController presentViewController:self.previewViewController animated:YES completion:^{
        NSLog(@"DIsplay complete!");
    }];
}

// MARK: delegate RPPreviewViewControllerDelegate
- (void)previewControllerDidFinish:(RPPreviewViewController*)previewController
{
    if (previewController != NULL) {
        [previewController dismissViewControllerAnimated:YES completion:^{
            
        }];
    }
}

// MARK: RPScreenRecorderDelegate
- (void)screenRecorder:(RPScreenRecorder *)screenRecorder didStopRecordingWithError:(NSError *)error previewViewController:(nullable RPPreviewViewController *)previewViewController
{
    // Display the error the user to alert them that the recording failed.
//    showScreenRecordingAlert(error.localizedDescription)
    
    /// Hold onto a reference of the `previewViewController` if not nil.
    if (previewViewController != NULL) {
        self.previewViewController = previewViewController;
    }
}

@end