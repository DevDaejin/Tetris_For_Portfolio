using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetriminoData : MonoBehaviour
{
    public static bool[][,] IArray = new bool[][,]
    {
        new bool[4,4]
        {
            { false,    false,  false,  false },
            { true,     true,   true,   true  },
            { false,    false,  false,  false },
            { false,    false,  false,  false }
        },
        new bool[4,4]
        {
            { false,    true,  false,  false },
            { false,    true,  false,  false },
            { false,    true,  false,  false },
            { false,    true,  false,  false }
        },
         new bool[4,4]
        {
            { false,    false,  false,  false },
            { false,    false,  false,  false },
            { true,     true,   true,   true  },
            { false,    false,  false,  false }
        },
        new bool[4,4]
        {
            { false,    false,  true,    false },
            { false,    false,  true,    false },
            { false,    false,  true,    false },
            { false,    false,  true,    false }
        }
    };

    public static bool[][,] OArray = new bool[][,]
    {
        new bool[2,2]
        {
            {true,   true},
            {true,   true}
        }
    };

    public static bool[][,] TArray = new bool[][,]
    {
        new bool[3,3]
        {
            { false,    false,  false   },
            { true,     true,   true    },
            { false,    true,   false   }
        },
        new bool[3,3]
        {
            { false,    true,   false   },
            { true,     true,   false   },
            { false,    true,   false   }
        },
        new bool[3,3]
        {
            { false,    true,   false   },
            { true,     true,   true    },
            { false,    false,  false   }
        },
        new bool[3,3]
        {
            { false,    true,   false   },
            { false,    true,   true    },
            { false,    true,   false   }
        }
    };

    public static bool[][,] SArray = new bool[][,]
    {
        new bool[3,3]
        {
            { false,    true,   true    },
            { true,     true,   false   },
            { false,    false,  false   }
        },
        new bool[3,3]
        {
            {true,      false,    false },
            {true,      true,     false },
            {false,     true,     false }
        }
    };

    public static bool[][,] ZArray = new bool[][,]
    {
        new bool[3,3]
        {
            { true,     true,   false   },
            { false,    true,   true    },
            { false,    false,  false   }
        },
        new bool[3,3]
        {
            { false,    false,    true,   },
            { false,    true,     true,   },
            { false,    true,     false,  }
        }
    };

    public static bool[][,] LArray = new bool[][,]
    {
        new bool[3,3]
        {
            {false,     false,  false   },
            {true,      true,   true    },
            {true,      false,  false   }
        },
        new bool[3,3]
        {
            { true,     true,   false   },
            { false,    true,   false   },
            { false,    true,   false   }
        },
        new bool[3,3]
        {
            { false,    false,  true,   },
            { true,     true,   true,   },
            { false,    false,  false   }
        },
        new bool[3,3]
        {
            { false,    true,   false,},
            { false,    true,   false,},
            { false,    true,   true, }
        }
    };

    public static bool[][,] JArray = new bool[][,]
    {
        new bool[3,3]
        {
            {true,      false,  false   },
            {true,      true,   true    },
            {false,     false,  false   },
        },
        new bool[3,3]
        {
            { false,    true,   true,   },
            { false,    true,   false,  },
            { false,    true,   false,  },
        },
        new bool[3,3]
        {
            { false,    false,  false   },
            { true,     true,   true    },
            { false,    false,  true    }
        },
        new bool[3,3]
        {
            { false,    true,   false   },
            { false,    true,   false   },
            { true,     true,   false   }
        }
    };
}
