﻿namespace PlasmaShaftCore
{
    public enum OpCode
    {
        #region Classic Protocol
        ServerIdentification = 0x00,
        Ping = 0x01,
        LevelInitialize = 0x02,
        LevelDataChunk = 0x03,
        LevelFinalize = 0x04,
        Blockchange = 0x06,
        SpawnPlayer = 0x07,
        PositionRotSpawn = 0x08,
        PositionRotUpdate = 0x09,
        PositionUpdate = 0x0a,
        OrientationUpdate = 0x0b,
        DespawnPlayer = 0x0c,
        Message = 0x0d,
        Disconnect = 0x0e,
        UpdateUserType = 0x0f,
        #endregion

        #region Classic Protocol Extended
        ExtInfo = 0x10,
        ExtEntry = 0x11,
        ClickDistance = 0x12,
        CustomBlocks= 0x13,
        HeldBlock = 0x14,
        TextHotKey = 0x15,
        ExtAddPlayerName = 0x16,
        ExtRemovePlayerName = 0x18,
        EnvSetColor = 0x19,
        MakeSelection = 0x1A,
        RemoveSelection = 0x1B,
        SetBlockPermission = 0x1C,
        ChangeModel = 0x1D,
        EnvSetMapAppearance = 0x1E,
        EnvWeatherType = 0x1F,
        HackControl = 0x20,
        ExtAddEntityV1 = 0x21,
        PlayerClicked = 0x22
        #endregion
    }
}
