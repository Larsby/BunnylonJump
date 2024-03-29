using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PastilleConstants
{


    public const string adInfoUrl = "https://www.pastille.se/ads/kicka_ads.txt";
    public const bool isDebugging = false;
    public const string applovinsdkkey = "cQiOh1amPJ0ywVME6E3S8a-vk89aDO0wNwrXM989beVQSsaiqq9EoKn2HJX6-VO3DZgpudHWb2J7RwmNMfTZoA";

    public const string uniqueID = "hoppaKaninen";
    public const string addScoreURL = "shuriken.se/bunnylonjumpscore/addscore.php?"; //be sure to add a ? to your url
    public const string highscoreURL = "shuriken.se/bunnylonjumpscore/display.php?uniqueID=hoppaKaninen";
    public const string getPositionURL = "shuriken.se/bunnylonjumpscore/getPosition.php?uniqueID=hoppaKaninen&name=";  //TODO refactor so that the uniqueID is not smack in the middle
    public const string secretKey = "kaninhopparen"; // Edit this value and make sure it's the same as the one stored on the server


}
