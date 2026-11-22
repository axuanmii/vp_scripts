/**
 *
 * Don't fit media. Media imported will be centered at its existing 
 * resolution and size with no relation to the video resolution and size.
 * The script will operate on selected video events. If none are selected,
 * a reminder will show up.
 * 
 **/

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ScriptPortal.Vegas;

public class EntryPoint
{

    public void FromVegas(Vegas vegas)
    {
        double dWidthProject  = vegas.Project.Video.Width;
        double dHeightProject = vegas.Project.Video.Height;
        double dPixelAspect   = vegas.Project.Video.PixelAspectRatio;
        double dAspect        = dPixelAspect * dWidthProject / dHeightProject;

        List<VideoEvent> events = new List<VideoEvent>();
        AddSelectedVideoEvents(vegas, events);
        if (0 == events.Count)
        {
            MessageBox.Show("No events have been selected yet");
        }

        foreach (VideoEvent videoEvent in events)
        {
            Take take = videoEvent.ActiveTake;
            if (null == take) continue;
            VideoStream videoStream = take.MediaStream as VideoStream;
            if (null == videoStream) continue;
            double dMediaPixelAspect = videoStream.PixelAspectRatio;
            foreach (VideoMotionKeyframe keyframe in videoEvent.VideoMotion.Keyframes)
            {
                UndoCropPanRotateResize(keyframe, dMediaPixelAspect, dAspect, dWidthProject, dHeightProject);                                                                     
            }
        }
    }

    void AddSelectedVideoEvents(Vegas vegas, List<VideoEvent> events)
    {
        foreach (Track track in vegas.Project.Tracks)
        {
            if (track.IsVideo())
            {
                foreach (VideoEvent videoEvent in track.Events)
                {
                    if (videoEvent.Selected)
                    {
                        events.Add(videoEvent);
                    }
                }
            }
        }
    }

/* 
 void AddAllVideoEvents(Vegas vegas, List<VideoEvent> events)
    {
        foreach (Track track in vegas.Project.Tracks)
        {
            if (track.IsVideo())
            {
                foreach (VideoEvent videoEvent in track.Events)
                {
                    events.Add(videoEvent);
                }
            }
        }
    }
*/
    void UndoCropPanRotateResize(VideoMotionKeyframe keyframe, double dMediaPixelAspect, double dAspectOut, double dWidthProject, double dHeightProject)
    {
        VideoMotionKeyframe keyframeSave = keyframe;
        try
        {
            double rotation = keyframe.Rotation;    
            
            // undo rotation so that we can get at correct aspect ratio.
            //
            keyframe.RotateBy(-rotation);

            // double dWidth         = Math.Abs(keyframe.TopRight.X   - keyframe.TopLeft.X);
            // double dHeight        = Math.Abs(keyframe.BottomLeft.Y - keyframe.TopLeft.Y);
            // double dCurrentAspect = dMediaPixelAspect * dWidth / dHeight;
            double centerY        = keyframe.Center.Y;
            double centerX        = keyframe.Center.X;        
        
            // double dFactor;
        
            VideoMotionBounds bounds = new VideoMotionBounds(keyframe.TopLeft, keyframe.TopRight, keyframe.BottomRight, keyframe.BottomLeft);
			
			bounds.TopLeft.Y     = (float) ((centerY - dHeightProject/2));
            bounds.TopRight.Y    = (float) ((centerY - dHeightProject/2));
            bounds.BottomLeft.Y  = (float) ((centerY + dHeightProject/2));
            bounds.BottomRight.Y = (float) ((centerY + dHeightProject/2));
			
			bounds.TopLeft.X     = (float) ((centerX - dWidthProject/2));
            bounds.TopRight.X    = (float) ((centerX + dWidthProject/2));
            bounds.BottomLeft.X  = (float) ((centerX - dWidthProject/2));
            bounds.BottomRight.X = (float) ((centerX + dWidthProject/2));
        
            // set it to new bounds
            keyframe.Bounds = bounds;
        
            // restore rotation.        
            keyframe.RotateBy (rotation);
        
        }
        catch (Exception e)
        {
            // restore original settings on error
            keyframe = keyframeSave;
        }    
    }

}


