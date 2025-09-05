# Changelog

## 0.2.1 - 2025-09-05
- 默认打包路径改为Application.streamingAssets
- 打AssetBundle代码中的路径改为直接使用 AssetConstPath 中的定义
- 打AssetBundle的方法 BuildAllAssetBundles() 改为 public

## 0.2.0 - 2025-09-05
- 添加完整Log并可设置是否开启Log
- 解决开始异步加载后立即释放的问题
- 解决开始异步加载后立即又同步加载的问题
- AssetBundleLoader异步加载的回调在bundle及其依赖均加载完成后再调用
- AssetManager只允许Init一次

## 0.1.7 - 2025-09-04
- 修复在bundle未加载完成时，再次请求加载的回调会立即返回的问题（没有等加载完成）

## 0.1.6 - 2025-07-10
- 修复打包时提示的编译报错

## 0.1.5 - 2025-07-10
- 修改AssetManager的初始化方法，可以外部传入AB的路径

## 0.1.4 - 2025-07-09
- 修复AssetManager在构建AB时的编译报错
- AssetManager添加主动初始化方法

## 0.1.3 - 2025-05-14
- AssetManager添加字段UseAB，可以设置是否使用AssetBundle

## 0.1.2 - 2025-01-16
- 修复AssetBundleLoader中某个局部变量提示重复定义的问题

## 0.1.1 - 2025-01-16
- 删除接口 IAssetLoader 和 IAssetBundleLoader 的方法的public修饰符

## 0.1.0 - 2025-01-16
- 初始版本
