import { IQuiz } from "./types";

const quiz: IQuiz = {
    "title": "test 1",
    "questions": [
        {
            "text": "HOTSPOT -<br>You have developed a Web App for your company. The Web App provides services and must run in multiple regions.<br>You want to be notified whenever the Web App uses more than 85 percent of the available CPU cores over a 5 minute period. Your solution must minimize costs.<br>Which command should you use? To answer, select the appropriate settings in the answer area.<br>NOTE: Each correct selection is worth one point.",
            "optionsContainer": {
                "parts": [
                    {
                    "type": "Text",
                    "value": "az monitor metrics alert create -n myAlert -g myResourceGroup --scopes targetResourcelD --condition \""
                }, {
                    "type": "OptionsGroup",
                    "value": 0
                }, {
                    "type": "Text",
                    "value": "> 85\""
                }, {
                    "type": "OptionsGroup",
                    "value": 1
                }, {
                    "type": "Text",
                    "value": "5m\""
                }],
                "groups": [
                    {
                        "itemsContainer": {
                            "options": [
                                {
                                    "text": "CPU Usage",
                                    "isCorrect": false
                                },
                                {
                                    "text": "Percentage CPU",
                                    "isCorrect": false
                                },
                                {
                                    "text": "avg Percentage CPU",
                                    "isCorrect": true
                                }
                            ]
                        }
                    },
                    {
                        "itemsContainer": {
                            "options": [
                                {
                                    "text": "--window-size",
                                    "isCorrect": true
                                },
                                {
                                    "text": "--evaluation-frequency",
                                    "isCorrect": false
                                },
                                {
                                    "text": "--auto-mitigate",
                                    "isCorrect": false
                                }
                            ]
                        }
                    }
                ]
            },
            "questionType": "TemplatedChoice"
        },
        {
            "text": "You have two Hyper-V hosts named Host1 and Host2. Host1 has an Azure virtual machine named VM1 that was deployed by using a custom Azure Resource<br>Manager template.<br>You need to move VM1 to Host2.<br>What should you do?<br>",
            "optionsContainer": {
                "options": [
                    {
                        "text": "From the Update management blade, click Enable.",
                        "isCorrect": false
                    },
                    {
                        "text": "From the Overview blade, move VM1 to a different subscription.",
                        "isCorrect": false
                    },
                    {
                        "text": "From the Redeploy blade, click Redeploy.",
                        "isCorrect": true
                    },
                    {
                        "text": "From the Profile blade, modify the usage location.",
                        "isCorrect": false
                    }
                ]
            },
            "questionType": "SimpleChoice"
        },
        {
            "text": "HOTSPOT -<br>You are developing an Azure Function App by using Visual Studio. The app will process orders input by an Azure Web App. The web app places the order information into Azure Queue Storage.<br>You need to review the Azure Function App code shown below.",
            "optionsContainer": {
                "parts": [
                    {
                        "type": "Text",
                        "value": "1&nbsp;"
                    }, {
                        "type": "OptionsGroup",
                        "value": 0
                    }, {
                        "type": "Text",
                        "value": "<br>2&nbsp;"
                    }, {
                        "type": "OptionsGroup",
                        "value": 1
                    }, {
                        "type": "Text",
                        "value": "<br>3&nbsp;"
                    }, {
                        "type": "OptionsGroup",
                        "value": 2
                    }],
                "groups": [
                    {
                        "itemsContainer": {
                            "options": [
                                {
                                    "text": "Yes",
                                    "isCorrect": true
                                },
                                {
                                    "text": "No",
                                    "isCorrect": false
                                }
                            ]
                        }
                    },
                    {
                        "itemsContainer": {
                            "options": [
                                {
                                    "text": "Yes",
                                    "isCorrect": true
                                },
                                {
                                    "text": "No",
                                    "isCorrect": false
                                }
                            ]
                        }
                    },
                    {
                        "itemsContainer": {
                            "options": [
                                {
                                    "text": "Yes",
                                    "isCorrect": false
                                },
                                {
                                    "text": "No",
                                    "isCorrect": true
                                }
                            ]
                        }
                    }
                ]
            },
            "questionType": "TemplatedChoice"
        },
        {
            "text": "You have two Hyper-V hosts named Host1 and Host2. Host1 has an Azure virtual machine named VM1 that was deployed by using a custom Azure Resource<br>Manager template.<br>You need to move VM1 to Host2.<br>What should you do?<br>",
            "optionsContainer": {
                "isOrdered": true,
                "options": [
                    {
                        "text": "From the Update management blade, click Enable.",
                        "isCorrect": true,
                        "order": 0
                    },
                    {
                        "text": "From the Overview blade, move VM1 to a different subscription.",
                        "isCorrect": false
                    },
                    {
                        "text": "From the Redeploy blade, click Redeploy.",
                        "isCorrect": true,
                        "order": 1
                    },
                    {
                        "text": "From the Profile blade, modify the usage location.",
                        "isCorrect": false
                    }
                ]
            },
            "questionType": "DragDropChoice"
        }
    ]
}