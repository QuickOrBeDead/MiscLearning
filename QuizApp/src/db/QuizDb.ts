import { Quiz } from "../types";

export class QuizDb
{
    indexedDB: IDBFactory;
    dbTransaction: IDBTransaction;
    db: IDBDatabase | undefined

    constructor() {
        this.indexedDB = this.getDbFactory()
        this.dbTransaction = this.getDbTransaction()

        if (!this.indexedDB) {
            alert("Your browser doesn't support a stable version of IndexedDB.")
        }
    }

    init(callback: () => void) {
        const dbOpenRequest = this.indexedDB.open("quizDb", 1)
        const that = this
        dbOpenRequest.onsuccess = function() {
            that.db = dbOpenRequest.result
            
            callback()
        }

        dbOpenRequest.onupgradeneeded = function(event: any) {
            const db = event.target.result
            if (!db.objectStoreNames.contains("quiz")) {
				db.createObjectStore("quiz", { keyPath: "id", autoIncrement:true })
			}
        }  
    }

    getQuizes(callback: (t: Quiz[]) => void) {
        const s = this.getQuizStore("readonly")
        const result: Quiz[] = []

        s.openCursor().onsuccess = function(event: any) {  
            var cursor = event.target.result;  
            if (cursor) {  
                console.log(cursor.key)
                result.push(cursor.value)
                cursor.continue() 
            }  
            else {  
                callback(result)
            }  
        }
    }

    addQuiz(quiz: Quiz, callback: (t: number) => void) {
        const s = this.getQuizStore("readwrite")
        const addRequest = s.add(quiz)
        addRequest.onsuccess = function(event: any) {
            const addedKey = event.target.result as number
            callback(addedKey)
        }

        addRequest.onerror = function(event: any) {
            console.log("Add request error:", event.target.error);
        }
    }

    getQuiz(id: number, callback: (t: Quiz) => void) {
        const s = this.getQuizStore("readonly")
        const getRequest = s.get(id)
        getRequest.onsuccess = function(event: any) {
            const item = event.target.result as Quiz
            callback(item)
        }
    }

    private getQuizStore(mode?: IDBTransactionMode | undefined) {
        const transaction = this.db!.transaction(["quiz"], mode)
        return transaction.objectStore("quiz")
    }

    getDbFactory = function(): IDBFactory {
        return window.indexedDB || (window as any)["webkitIndexedDB"] as IDBFactory || (window as any)["mozIndexedDB"] || (window as any)["msIndexedDB"]
    }

    getDbTransaction = function(): IDBTransaction {
        return (window as any)["IDBTransaction"] || (window as any)["webkitIDBTransaction"] as IDBTransaction
    }
}