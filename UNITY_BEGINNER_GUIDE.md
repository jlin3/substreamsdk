# 👶 Unity Setup for Complete Beginners

## Before You Start

**You need:**
- Unity Hub installed (https://unity.com/download)
- A Unity account (free)
- 5 minutes

## Step 1: Open Unity Hub

Double-click the Unity Hub icon on your desktop/dock

## Step 2: Create New Project

1. Look for **"New project"** button (usually top-right)
2. You'll see project templates - pick **"3D"**
3. At the bottom:
   - **Project name**: Type `SubstreamDemo`
   - **Location**: Leave as default (or click ... to choose)
4. Click the blue **"Create project"** button
5. ☕ Wait 1-2 minutes for Unity to open

## Step 3: Get the Files

1. **In your web browser**, go to:
   ```
   https://github.com/jlin3/substreamsdk
   ```

2. **Download these 2 files**:
   - `SubstreamSDK_WithProminentViewer.zip` (click to download)
   - `test-scene.unitypackage` (click to download)
   
   They'll go to your Downloads folder

## Step 4: Import to Unity

1. **In Unity** (should be open now), look at the top menu
2. Click: **Assets → Import Package → Custom Package**
3. Go to Downloads folder, select `test-scene.unitypackage`
4. A window pops up - click **"Import"**

5. **Now the SDK**:
   - Find `SubstreamSDK_WithProminentViewer.zip` in Downloads
   - Double-click to unzip it
   - You'll see folders: Scripts, Editor, Plugins
   
6. **In Unity's Project window** (bottom of screen):
   - See the "Assets" folder?
   - Drag the 3 folders (Scripts, Editor, Plugins) onto "Assets"
   - Unity will say "Importing..." for a few seconds

## Step 5: Create the Demo

1. Look at Unity's top menu again
2. Click: **GameObject**
3. You should now see: **Substream** (new!)
4. Click: **GameObject → Substream → Create Demo Scene with UI**
5. Stuff appears in your scene! ✨

## Step 6: Test It!

1. Find the **Play button** ▶️ at the top-center of Unity
2. **Click Play** - Unity enters play mode
3. Look at the Game window - you see:
   ```
   SUBSTREAM LIVE DEMO
   [🎮 START STREAMING]
   ```
4. **Click the green START STREAMING button**
5. A blue panel appears with **[📺 OPEN VIEWER]**
6. **Click OPEN VIEWER** - your browser opens!
7. In the browser, click **"Join"** to watch your stream

## You Did It! 🎉

You're now streaming from Unity to the web!

## Common Questions

**Q: Where are the Game/Scene windows?**
- Top of the center panel has tabs: Scene | Game
- Click "Game" to see what players see

**Q: How do I stop?**
- Click the Play button ▶️ again to stop
- Or click the red STOP STREAMING button

**Q: Where's the Console?**
- Window → General → Console
- Shows helpful messages

**Q: Can I move the windows around?**
- Yes! Drag the tabs to rearrange
- Unity saves your layout

## What's Next?

- Try the keyboard shortcuts: S (start), X (stop), V (view)
- Customize the UI colors and text
- Build for Quest (requires Android setup)

## Still Stuck?

The most common issues:
1. **Forgot to import Editor folder** → No Substream menu
2. **Didn't press Play** → Nothing happens
3. **Browser blocked popup** → Manually open viewer link

---

Remember: Everyone was a beginner once. You've got this! 💪
