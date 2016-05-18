using UnityEngine;
using System.Collections.Generic;


public static class GameDef
{
    #region ids
    public const int INVAILD_ID = -1;
    public const int INVAILD_TABLE_ID = 0;
    #endregion

    #region common assets
    public const string ACTION_FILE = "table/action_setup.tbl";
    public const string HPTOP_UI = "battle_state";
    #endregion

    #region layers
    public static int OBSTACLE_LAYER = LayerMask.NameToLayer("Block");
    public static int TERRAIN_LAYER = LayerMask.NameToLayer("Terrain");
    public static int UI_LAYER = LayerMask.NameToLayer("UI");
    public static int UIDIALOG_LAYER = LayerMask.NameToLayer("UIDialog");
    public static int UNIT_LAYER = LayerMask.NameToLayer("Unit");
    public static int WALL_LAYER = LayerMask.NameToLayer("Wall");
    public static int EXCAMERA_LAYER = LayerMask.NameToLayer("ExCamera");
    #endregion

    #region common node
    public const string NODE_HEAD = "_head";
    public const string NODE_DAMAGE = "Bip001";
    public const string NODE_CAMERA = "CameraSlot";
    #endregion

    #region buff def
    public const int BuffStateImmune = 1 << 0;          // 免疫
    public const int BuffStateStun = 1 << 1;            // 眩晕
    public const int BuffStateSilence = 1 << 2;         // 沉默
    public const int BuffStateShieldDamage = 1 << 3;    // 煞气护盾
    public const int BuffStateShieldDamageHp = 1 << 4;  // hp护盾
    public const int BuffStateReflectDamage = 1 << 5;   // 反伤
    public const int BuffFocusMove = 1 << 6;            // 强制移动
    public const int PowerMaxState = 1 << 7;            // 爆气状态
    public const int QTEState = 1 << 8;                 // QTE状态

    // buff起效类型
    public enum EBuffAffectType
    {
        TypeNone = 0,
        TypeTime = 1,               // 时长型
        TypeFrequency = 2,          // 频率型
    }

    // buff取消方式
    public enum EBuffRemoveType
    {
        TypeNone = 0,
        TypeTimeReach = 1,              // 时间解除
        TypeDeath = 2,                  // 死亡解除
        TypeBeHitted = 4,               // 伤害解除
        TypeLoadScene = 8,              // 场景解除
    }

    public enum EBuffAddType
    {
        TypeTogether = 0,
        TypeReplace,
        TypeAdd,
    }

    public enum  EBuffEffectActiveType
    {
        TypeNone = 0,
        TypeStart,
        TypeEnd,
        TypeUpdate,
        TypeAttack,
        TypeAction,
        TypeBehit,
        TypeRingEnter,
        TypeRingExit,
        TypeInRing,

    }
    #endregion

    #region app stages
    public enum EGameStage
    {
        startup_stage = 1,
    };
    #endregion

    #region unit def
    public enum EUnitMainType
    {
        none,
        hero=1,
        npc=2,
        monster=3,
        sceneobject=10,
        exsummon=11,
        loot=12,
    }

    /// <summary>
    /// 召唤物子类型
    /// </summary>
    public enum EUnitSummonSubType
    {
        none,
        bullet,
    }

    /// <summary>
    /// 掉落物子类型
    /// </summary>
    public enum EUnitLootSubType
    {
        none,
        energe, // 能量球
    }

    public enum EUnitType
    {
        None,
        Actor,
        MainPlayer,
        SceneObject,
        Monster,
        Bullet,
        EnergeLoot,
    };

    public enum EUnitCamp
    {
        EUC_NONE = 0,	    // 中立
        EUC_SELF = 1,       // 我方
        EUC_ENEMY = 2,      // 敌人
        EUC_FRIEND = 3,     // 友方
    };

    // 属性类型
    public enum EAttrType
    {
        TypeMaxHp = 1,                  // 生命值上限
        TypeMoraleAtk = 2,              // 士气攻击
        TypeCriticalPer = 3,            // 暴击率
        TypeCritAttack = 4,             // 暴击伤害
        TypeMoraleDefense = 5,          // 防御
        TypeMoveSpeed = 6,              // 移动速度
        TypeMorale = 7,                 // 士气值
        TypeMoraleCharge = 8,           // 士气恢复速度
        TypeCrashAtk = 9,               // 撞击伤害
        TypeMoraleRes = 10,             // 伤害抗性
        TypeHpRecover = 11,             // 生命回复
        TypeCurrentHp = 12,             // 当前血量
        TypeMaxBaseMorale = 13,         // 基础士气值
        TypeAcMoraleCharge = 14,        // 士气恢复加速度
        TypeMoraleToRecover = 15,       // 当前恢复的士气值（用这个值判断是否恢复满）
        TypeSha = 16,                   // 煞气值
        TypeReflect = 17,               // 反伤概率
        TypeShaGetDistance = 18,        // 煞气拾取距离

        TypeMax,
    }

    public const string DEF_ANIMATION = "idle_ground";
    #endregion

    #region battle
    public enum ESkillType
    {
        normal,
        sp,
        command,
    }

    public enum ESkillTargetType
    {
        friend,
        enemy,
        all,
    }

    public enum ESkillHitType
    {
        single,
        back_single,
        random_single,
        row,
        column,
        back_column,
        all,
        random_all,
    }

    public enum EBattleFaction
    {
        player,
        enemy,
    }
    #endregion
}

