# 📦 Creating Clean Test Scene Package

## Steps to Create test-scene-v2.unitypackage

### 1. In Unity:

1. **Import the original test-scene.unitypackage**
2. **DELETE these conflicting files:**
   - `Assets/SubstreamSDK/Scripts/Substream.cs`
   - `Assets/SubstreamSDK/Demos/Scripts/DemoController.cs`
   - `Assets/SubstreamSDK/Scripts/` (entire folder if empty)
   - `Assets/SubstreamSDK/Demos/Scripts/` (entire folder if empty)

3. **Keep these assets:**
   - The test scene itself
   - Cube prefab
   - Particle effects
   - Materials
   - UI elements

4. **Create a README in the scene folder:**
   ```
   Assets/TestScene/README.txt
   
   SUBSTREAM TEST SCENE
   ===================
   
   This scene includes:
   - Rotating cube
   - Particle effects  
   - Score counter
   - UI canvas
   
   TO ADD STREAMING:
   1. Download SubstreamSDK.cs from the repo
   2. Add it to any GameObject in this scene
   3. Press Play and click "Go Live"!
   
   No other scripts needed!
   ```

5. **Export as new package:**
   - Select: Assets/TestScene (or wherever the scene assets are)
   - Right-click → Export Package
   - Name: `test-scene-v2.unitypackage`
   - Include dependencies

### 2. Test the Clean Package:

1. Create new Unity project
2. Import test-scene-v2.unitypackage
3. Add ONLY SubstreamOneClick.cs
4. Verify no conflicts!

### 3. Update Repository:

```bash
# Archive old package
mv test-scene.unitypackage archived-scripts/test-scene-old.unitypackage

# Add new package
# (copy test-scene-v2.unitypackage from Unity project)

# Update README to reference v2
```

## What This Solves:

- ✅ No more script conflicts
- ✅ Clean demo content  
- ✅ Works with new SDK
- ✅ Clear instructions
- ✅ No confusion for developers

The test scene becomes just visual content - developers bring their own streaming script!
