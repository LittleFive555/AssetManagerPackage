# AssetManager

这是一个用于管理Unity中Asset的包，它提供了在Editor和Runtime中加载和卸载Asset的功能。

## 功能

- 在Editor中创建AssetBundle
- 在Editor中检查AssetBundle的依赖关系（是否存在循环依赖）
- 在Runtime中加载和卸载Asset
- 在Runtime中检查AssetBundle的依赖关系

## 导入包

1. 在Unity中，选择菜单 `Window -> Package Manager`
2. 然后点击 `+` 按钮，选择 `Add package from git URL...`
3. 然后输入 `https://github.com/LittleFive555/AssetManagerPackage.git`，回车，即可导入包。

## 使用

### 构建AssetBundle
1. 手动配置资源的AssetBundle名称
2. 选择菜单 `Assets -> Build AssetBundles`，即可构建AssetBundle

### 加载资源
1. 使用AssetManager.LoadAsset<T>(path)即可加载资源
2. 使用AssetManager.LoadAssetAsync<T>(path, onComplete)即可异步加载资源
3. 使用AssetManager.UnloadAsset<T>(asset)即可卸载资源

