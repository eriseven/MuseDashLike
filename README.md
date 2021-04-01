# MuseDashLike

打开TimeLine编辑器

![](https://raw.githubusercontent.com/eriseven/pictures/main/pic-go-img/20210316094648.png)

选中测试场景中的音乐配置文件

![](https://raw.githubusercontent.com/eriseven/pictures/main/pic-go-img/20210315142958.png)

音轨编辑器界面

![](https://raw.githubusercontent.com/eriseven/pictures/main/pic-go-img/20210315095011.png)


从右键菜单添加音频轨道和音符轨道，需要创建两个音符轨道，第一个是左测轨道，第二个是右侧轨道

![](https://raw.githubusercontent.com/eriseven/pictures/main/pic-go-img/20210316095035.png)


在音符轨道上创建操作音符，目前支持3种
Muse Click Note：单机
Muse Muliti Click Note：连击
Muse Long Click Note：长按

![](https://raw.githubusercontent.com/eriseven/pictures/main/pic-go-img/20210315142831.png)

新增双击音符
Muse Double Click Note：双击音符，必须设置在左音轨，运行时会在左右音轨同时生成一对双击判定音符
![](https://raw.githubusercontent.com/eriseven/pictures/main/pic-go-img/20210401092205.png)
![](https://raw.githubusercontent.com/eriseven/pictures/main/pic-go-img/20210401092412.png)
将音频剪辑拖入音频轨道

![](https://raw.githubusercontent.com/eriseven/pictures/main/pic-go-img/20210315142700.png)

Muse Click Note
Perfect Offset Time ：完美操作判定配置，0.5代表在音符精确位置的0.5个单位范围内，即可判定为完美操作，下面的配置以此类推
Good Offset Time ：良好操作判定配置
Success Offset Time ：普通成功操作判定配置

![](https://raw.githubusercontent.com/eriseven/pictures/main/pic-go-img/20210401093048.png)

Muse Multi Click Note

Click Count：配置需要点击的次数

Perfect Click Count：完美操作判定配置，5代表，完成连续5次点击，即可判定为完美操作。
Good Click Count: 良好操作盘底那个配置
Success Click Count: 普通操作成功判定配置
![](https://raw.githubusercontent.com/eriseven/pictures/main/pic-go-img/20210315143108.png)


![](https://raw.githubusercontent.com/eriseven/pictures/main/pic-go-img/20210401093529.png)

Muse Long Click Note

Perfect Percent: 完美操作判定成功配置，百分比，0.9代表，持续长按时间超过80%就判定为完美操作
Good Percent: 良好操作判定配置。
Success Percent：普通操作成功判定


![](https://raw.githubusercontent.com/eriseven/pictures/main/pic-go-img/20210315143226.png)

![](https://raw.githubusercontent.com/eriseven/pictures/main/pic-go-img/20210401095241.png)


得分配置
![](https://raw.githubusercontent.com/eriseven/pictures/main/pic-go-img/20210401095502.png)
