//
//  ReplayKitProxy.swift
//  ReplayKitDemo
//
//  Created by Tzt on 15/12/11.
//  Copyright © 2015年 Tzt. All rights reserved.
//

#import <UIKit/UIKit.h>
#import <ReplayKit/ReplayKit.h>

@interface ReplayKitProxy : NSObject<RPPreviewViewControllerDelegate, RPScreenRecorderDelegate>

+ (ReplayKitProxy*) sharedInstance;

// 是否支持录像功能(仅ios9以上支持)
- (BOOL)isSupportReplay;

// 开始录制视频
- (void)startRecording;

// 停止录制视频
- (void)stopRecording;

// 删除已录制视频，必须在stopRecording之后调用(eg.离开视频分享界面)
- (void)discardRecording;

// 显示视频
- (void)displayRecordingContent;

@end