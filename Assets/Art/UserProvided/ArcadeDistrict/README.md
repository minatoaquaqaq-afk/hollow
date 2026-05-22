# Arcade District User Art

把你这次发的四张原始 PNG 放到 `Source` 目录，并使用下面的文件名：

- `reference.png`：第 1 张，带 UI 的总参考图。
- `background.png`：第 2 张，纯背景图。
- `props_1.png`：第 3 张，街机/标牌/装饰素材图。
- `props_2.png`：第 4 张，地砖/墙体/门/结构素材图。

然后运行：

```powershell
cd G:\HollowStyleMVP
& "C:\Users\csc2bjy\.cache\codex-runtimes\codex-primary-runtime\dependencies\python\python.exe" Tools\build_user_arcade_art.py
```

生成结果：

- `arcade_background.png`
- `reference_overlay.png`
- `Components/*.png`
- `Previews\layered_testroom_preview.png`

Unity 场景生成器会优先使用这里的素材，不再使用之前 image-gen 自己画的素材。

