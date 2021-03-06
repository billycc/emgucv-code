//----------------------------------------------------------------------------
//  Copyright (C) 2004-2016 by EMGU Corporation. All rights reserved.       
//----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

#if !(__IOS__ || UNITY_IPHONE || UNITY_ANDROID || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN || NETFX_CORE)
using Emgu.CV.Cuda;
#endif
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.Util;

namespace Emgu.CV.Stitching
{
   /// <summary>
   /// Image Stitching.
   /// </summary>
   public partial class Stitcher : UnmanagedObject
   {
      /// <summary>
      /// The stitcher statis
      /// </summary>
      public enum Status
      {
         /// <summary>
         /// Ok.
         /// </summary>
         Ok = 0,
         /// <summary>
         /// Error, need more images.
         /// </summary>
         ErrNeedMoreImgs = 1,
         /// <summary>
         /// Error, homography estimateion failed.
         /// </summary>
         ErrHomographyEstFail = 2,
         /// <summary>
         /// Error, camera parameters adjustment failed.
         /// </summary>
         ErrCameraParamsAdjustFail = 3
      }

      /// <summary>
      /// Creates a stitcher with the default parameters.
      /// </summary>
      /// <param name="tryUseGpu">If true, the stitcher will try to use GPU for processing when available</param>
      public Stitcher(bool tryUseGpu)
      {
         _ptr = StitchingInvoke.cveStitcherCreateDefault(tryUseGpu);
      }

      /// <summary>
      /// Compute the panoramic images given the images
      /// </summary>
      /// <param name="images">The input images. This can be, for example, a VectorOfMat</param>
      /// <param name="pano">The panoramic image</param>
      /// <returns>The stitching status</returns>
      public Status Stitch(IInputArray images, IOutputArray pano)
      {
         using (InputArray iaImages = images.GetInputArray())
         using (OutputArray oaPano = pano.GetOutputArray())
            return StitchingInvoke.cveStitcherStitch(_ptr, iaImages, oaPano);
      }

      /// <summary>
      /// Set the features finder for this stitcher.
      /// </summary>
      /// <param name="finder">The features finder</param>
      public void SetFeaturesFinder(FeaturesFinder finder)
      {
         StitchingInvoke.cveStitcherSetFeaturesFinder(_ptr, finder.Ptr);
      }

      /// <summary>
      /// Release memory associated with this stitcher
      /// </summary>
      protected override void DisposeObject()
      {
         StitchingInvoke.cveStitcherRelease(ref _ptr);
      }
   }

   public static partial class StitchingInvoke
   {
      
      static StitchingInvoke()
      {
#if !(__IOS__ || UNITY_IPHONE || UNITY_ANDROID || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN || NETFX_CORE)
         //Dummy code to make sure the static constructor of GpuInvoke has been called
         bool hasCuda = CudaInvoke.HasCuda;
#else		 
		 CvInvoke.CheckLibraryLoaded();
#endif		 
      }

      [DllImport(CvInvoke.ExternLibrary, CallingConvention = CvInvoke.CvCallingConvention)]
      internal static extern IntPtr cveStitcherCreateDefault(
         [MarshalAs(CvInvoke.BoolMarshalType)]
         bool tryUseGpu
         );

      [DllImport(CvInvoke.ExternLibrary, CallingConvention = CvInvoke.CvCallingConvention)]
      internal static extern Stitcher.Status cveStitcherStitch(IntPtr stitcherWrapper, IntPtr images, IntPtr pano);

      [DllImport(CvInvoke.ExternLibrary, CallingConvention = CvInvoke.CvCallingConvention)]
      internal static extern void cveStitcherSetFeaturesFinder(IntPtr stitcherWrapper, IntPtr finder);

      [DllImport(CvInvoke.ExternLibrary, CallingConvention = CvInvoke.CvCallingConvention)]
      internal static extern void cveStitcherRelease(ref IntPtr stitcherWrapper);
   }
}
