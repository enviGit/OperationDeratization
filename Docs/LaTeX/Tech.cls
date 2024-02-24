% PLIK KONFIGURACYJNY

\NeedsTeXFormat{LaTeX2e} %
\ProvidesClass{tech}

% DECLARING OPTIONS
\usepackage{listings}
\usepackage{amsmath}
\newcommand\tab[1][1cm]{\hspace*{#1}}

\DeclareOption*{\PassOptionsToClass{\CurrentOption}{article}}


% PROCESSING OPTIONS
\ProcessOptions
\LoadClass[a4paper,twoside]{article} % szablon gotowy do druku na papierze A4 dwustronnie - jak ktoś tego nie chce - zmieniać tutaj https://www.overleaf.com/learn/latex/Single_sided_and_double_sided_documents


\RequirePackage{setspace}
\RequirePackage{times}
\RequirePackage[T1]{fontenc}
\RequirePackage{graphicx}
\RequirePackage{geometry}
\RequirePackage[usenames]{color}
\RequirePackage{babel}
\RequirePackage{enumitem}

\geometry{bindingoffset=10mm, lmargin=20mm, rmargin=20mm, tmargin=25mm, bmargin=25mm, foot=10mm, head=10mm}


\renewcommand{\baselinestretch}{1.3}
\setlength{\parindent}{2em}
\setlength{\parskip}{1em}

\bibliographystyle{abbrv}

%------------------------------------------------------------------------------

%------------------------------------------------------------------------------

\setcounter{page}{5}

%\ProcessOptions
\frenchspacing
\sloppy
\widowpenalty     9999
\clubpenalty      9999
\flushbottom


% code listing settings:
\usepackage{xcolor}
\usepackage{cascadia-code}
\usepackage{adjustbox}
\usepackage{tcolorbox}
\usepackage{caption}
\definecolor{black}{cmyk}{0,0,0,1}             
\definecolor{white}{cmyk}{0,0,0,0}
\definecolor{darkgrey}{cmyk}{0,0,0,0.97}      %background in Visual Studio
\definecolor{green}{cmyk}{0.6,0,0.84,0}       %comments in Visual Studio
\definecolor{blue}{cmyk}{0.65,0.33,0,0.05}       %keywords in Visual Studio
\definecolor{rose}{RGB}{214, 157, 133}       %strings in Visual Studio
\definecolor{lavender}{cmyk}{0,0.42,0,0.1}   %if/else,switch/case etc. in Visual Studio
\definecolor{lightblue}{cmyk}{0.35,0,0,0}     %local variables in Visual Studio
\definecolor{aqua}{RGB}{112, 201, 176}       %class types in Visual Studio
\definecolor{lightgreen}{RGB}{220, 220, 170} %enumerations and methods in Visual Studio
\definecolor{grassgreen}{RGB}{184, 215, 163} %enumerations and methods in Visual Studio

\lstloadlanguages{csh, C++, C, Python}

\lstset{
language=[Sharp]C,
showtabs=false,
showspaces=false,
showstringspaces=true,
breaklines=true,
breakatwhitespace=true,
backgroundcolor=\color{darkgrey},
rulecolor=\color{darkgrey},
commentstyle=\color{green},
morecomment=[s][\color{green}]{/*+}{*/},
morecomment=[s][\color{green}]{/*-}{*/},
basicstyle=\ttfamily\footnotesize\color{white},
stringstyle=\color{rose},
%**********************
%ADD YOUR KEYWORDS HERE
%**********************
%-------------------------------------------
%keywords like string, int, false, true ...
keywordstyle=\color{blue},
%-------------------------------------------
%keywords like struct, vector3
emphstyle=\color{grassgreen},
emph={Vector3, RaycastHit, LayerMask, Stance, KeyCode, ForceMode, HumanBodyBones, AiState, AiStateId, Quaternion, PoolType, Mathf, IEnumerator},
%-------------------------------------------
%keywords like if/else, switch/case ...
emphstyle={[2]\color{lavender}},
emph={[2]if, else, return, throw, switch, case, while, foreach, do, for, in, break, yield},
%-------------------------------------------
%Collection of your local and global variables
emphstyle={[3]\color{lightblue}},
emph={[3]bone, boneWeight, targetPosition, i, j, hit, obstacleMask, rigidBody, force, agent, objectToSpawn, spawnPosition, spawnRotation, poolType, pool, p, spawnableObj, parentObject, obj, objName, index, asyncOperation, progress, elapsedTime, distanceToCamera, distance, dollyCart, pathPosition, count, target, indicator, instance, Instance, name, Name, OnAudioEvent, audioSource, timeToDespawn, _timerCoroutine, deltaTime, gameObject, transform, parent, indicatorPrefab, holder, Death, Idle, FindWeapon, AttackTarget, FindTarget, FindFirstAidKit, FindAmmo, Patrol, InvestigateSound, useEvents, prompt, OnInteract, ObjectPools, LookupString, InactiveObjects, position, rotation, Length, aimTransform, targetTransform, iterations, boneTransforms, humanBones, weight, rigidBodies, isKinematic, animator, enabled, isUsingCameraCheck, vCamera, m_PathPosition, allowSceneActivation, isDone, progressSlider, zero, identity, unRegister},
%-------------------------------------------
%Collection of your class types
emphstyle={[4]\color{aqua}},
emph={[4]WeaponIk, PlayerMotor, Input, Physics, PlayerStance, MonoBehaviour, Transform, Range, GameObject, Serializable, Ragdoll, Rigidbody, AiAgent, InteractionEvent, SerializeField, Interactable, Header, ObjectPoolManager, PooledObjectInfo, Debug, SceneLoader, Slider, AsyncOperation, SceneManager, Time, ReturnToPoolOnAnimationEnd, ReturnToPoolAfterTimer, Coroutine, ReturnParticlesToPool, CinemachineTrackedDolly, DoorMotionSensor, DISystem, DamageIndicator, RectTransform, AudioEventManager, Action, AudioSource},
%-------------------------------------------
%Collection of build-in methods
emphstyle={[5]\color{blue}},
emph={[5]Start, Awake, FixedUpdate, LateUpdate, var, OnEnable, OnParticleSystemStopped, get, set, partial, var, value},
%-------------------------------------------
%Collection of your methods
emphstyle={[6]\color{lightgreen}},
emph={[6]AimAtTarget, GetTargetPosition, Raycast, NameToPlayer, GetKeyDown, CrouchToggle, ActivateRagdoll, ApplyForce, GetBoneTransform, GetComponent, AddForce, Enter, Update, Exit, GetId, Invoke, Interact, BaseInteract, OnLook, SpawnObject, FirstOrDefault, Add, SetParentObject, Instantiate, SetParent, Remove, SetActive, ReturnObjectToPool, Substring, LogWarning, LoadScene, StartCoroutine, LoadScene_Coroutine, LoadSceneAsync, MoveTowards, DestroyParent, ReturnToPoolAfterTime, OnParticleSystemStopped, GetCinemachineComponent, Distance, Create, Countdown, FindGameObjectWithTag, AddComponent, NotifyAudioEvent},
}

\newtcolorbox{codebox}{
    colback=darkgrey,
    colframe=darkgrey,
    boxrule=0pt,
    left=5pt,
    right=5pt,
    top=5pt,
    bottom=5pt,
}