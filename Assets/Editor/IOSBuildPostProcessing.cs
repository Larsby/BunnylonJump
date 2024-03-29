
#if UNITY_IOS
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Build;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

using System.IO;

/* IOSBuildPostProcessing
 * 
 * Author: Mattias Öhman
 * 
 * Purpose: Make IOS deployment/building easier.
 *		 	Automatically adds capabilities and required frameworks for IOS. 
 * 			I.E Adds cloudsave and IAP and the frameworks required for AppLovin
 * 			Ads network. Also add an required compiler flag. 
 * 
 
 * Dependencies: Only works on Unity version from 2018.3 and up
 * 
 * Usage: 	Place in Editor folder inside the assets folder and Unity will 
 * 			automagically run it after it has created the xcode project.
 * 
 * 	
 *   
*/

public class IOSBuildPostProcessing : IPostprocessBuildWithReport
{

	public int callbackOrder { get { return 0; } }
	[PostProcessBuild]

	string entitlementData = @"
     <?xml version=""1.0"" encoding=""UTF-8\""?>
     <!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
     <plist version=""1.0"">
      <dict>
    <key>com.apple.developer.icloud-container-identifiers</key>
    <array/>
    <key>com.apple.developer.ubiquity-kvstore-identifier</key>
    <string>$(TeamIdentifierPrefix)$(CFBundleIdentifier)</string>
</dict>
     </plist>";

	void IPostprocessBuildWithReport.OnPostprocessBuild(BuildReport report)
	{

		if (report.summary.platform != BuildTarget.iOS)
			return;

		string[] requiredFrameworks = new string[] {
			"EventKit.framework",
			"EventKitUI.framework",
			"Security.framework",
			"MobileCoreServices.framework",
			"AdSupport.framework",
			"libz.tbd",
			"WebKit.framework",
			"SystemConfiguration.framework",
			"AVFoundation.framework",
			"StoreKit.framework",
			"UIKit.framework",
			"CoreMedia.framework",
			"CoreGraphics.framework",
			"CoreTelephony.framework",
			"SafariServices.framework",
			"Foundation.framework"
		};

		string pbxProjectPath = PBXProject.GetPBXProjectPath(report.summary.outputPath);
		PBXProject pbxProject = new PBXProject();
		pbxProject.ReadFromFile(pbxProjectPath);
		var targetName = PBXProject.GetUnityTargetName();
		string targetGuid = pbxProject.TargetGuidByName(targetName);

		foreach (var framework in requiredFrameworks)
		{
			if (!pbxProject.ContainsFramework(targetGuid, framework))
			{
				pbxProject.AddFrameworkToProject(targetGuid, framework, false);
			}
		}

		// -ObjC linker is required for AppLovin SDK
		pbxProject.AddBuildProperty(targetGuid, "OTHER_LDFLAGS", "-ObjC");
		var entitleFileName = "game.entitlements";
		var destinationPath = report.summary.outputPath + "/" + targetName + "/" + entitleFileName;
		System.IO.File.WriteAllText(destinationPath, entitlementData);
		pbxProject.AddFile(targetName + "/" + entitleFileName, entitleFileName);
		pbxProject.AddBuildProperty(targetGuid, "CODE_SIGN_ENTITLEMENTS", targetName + "/" + entitleFileName);
		pbxProject.AddCapability(targetGuid, PBXCapabilityType.InAppPurchase, destinationPath, true);
		pbxProject.WriteToFile(pbxProjectPath);


	}
}
#else
public class IOSBuildPostProcessing
{

}
#endif