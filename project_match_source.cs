/**
 *
 * match project size to selected source media size
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
        foreach (Track track in vegas.Project.Tracks)
        {
            if (track.IsVideo())
            {
                foreach (VideoEvent ev in track.Events)
                {
                    if (ev.Selected && ev.ActiveTake != null)
                    {
                        VideoStream vStream = ev.ActiveTake.MediaStream as VideoStream;
                        if (vStream != null)
                        {
                            vegas.Project.Video.Width = vStream.Width;
                            vegas.Project.Video.Height = vStream.Height;
                        }
                        return;
                    }
                }
            }
        }
    }
}