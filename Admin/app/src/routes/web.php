<?php

use App\Http\Controllers\AccountController;
use App\Http\Controllers\blockController;
use App\Http\Controllers\FollowController;
use App\Http\Controllers\gameManagementController;
use App\Http\Controllers\ItemListController;
use App\Http\Controllers\LandController;
use App\Http\Controllers\LoginController;
use App\Http\Controllers\logscontroller;
use App\Http\Controllers\mailController;
use App\Http\Controllers\stageController;
use App\Http\Controllers\UserItemListController;
use App\Http\Controllers\UserListController;
use App\Http\Middleware\AuthMiddleware;
use App\Http\Middleware\NoCacheMiddleware;
use Illuminate\Support\Facades\Route;


Route::middleware(NoCacheMiddleware::class)->group(function () {
//ログイン画面を表示する
    Route::get('/', [LoginController::class, 'index'])->name('/');
//echo md5('password');

//ログインする
    Route::post('/', [LoginController::class, 'dologin'])->name('login');

//管理画面を表示する
    Route::get('accounts/gameManagement',
        [gameManagementController::class, 'gameManagement'])->name('accounts.gameManagement');

//アカウント関連のルートをグループ化
    Route::prefix('accounts')->name('accounts')->controller(AccountController::class)->middleware(AuthMiddleware::class)->group(function (
    ) {
        Route::get('/', 'index')->name('index');          //一覧表示画面　accounts.index
        Route::post('/', 'index')->name('index');          //一覧表示画面　accounts.index
        //アカウント登録
        Route::get('create', 'create')->name('create');  //登録画面 accounts.create
        Route::post('store', 'store')->name('store');      //登録処理 accounts.store
        Route::get('completion', 'completion')->name('completion');//登録完了画面 accounts.completion
        Route::post('search', 'search')->name('search');  //検索処理 accounts.search

        //アカウント削除
        Route::post('account_destroy', 'account_destroy')->name('account_destroy');//アカウント削除確認画面
        Route::post('destroy', 'destroy')->name('destroy');//アカウント削除処理　accounts.destroy
        Route::get('destroy_complete',
            'destroy_complete')->name('destroy_complete');//アカウント削除完了　accounts.destoroycomplete

        //パスワード更新
        Route::post('password_update', 'password_update')->name('password_update');//パスワード更新画面
        Route::post('update', 'update')->name('update');//パスワード更新処理　accounts.update
        Route::get('update_complete', 'update_complete')->name('update_complete');//パスワード更新完了処理 accounts.update_complete
    });

    //メール関連のルートをグループ化
    Route::prefix('mails')->name('mails')->controller(mailController::class)->group(function () {
        Route::get('mailmaster', 'mail_index')->name('mail_index');//メールマスタ一覧
        Route::get('user_mailList', 'user_mailList')->name('user_mail_list');//メール受信一覧を表示
        Route::get('mail_send', 'show_send')->name('mail_send');//メール送信の表示
        Route::post('mail_send', 'send')->name('mail_send');//メール送信処理
    });

    //フォローのルートをグループ化
    Route::prefix('follows')->name('follows')->controller(FollowController::class)->group(function () {
        //フォローリストを表示する
        Route::get('follow', 'ShowFollowList')->name('follow_List');

    });
    //フォローログのルートをグループ化
    Route::prefix('follow_logs')->name('follow_logs')->controller(logscontroller::class)->group(function () {
        //フォローのログを表示する
        Route::get('logs', 'show_follow_log')->name('logs');
    });

    //ステージのルートをグループ化
    Route::prefix('stages')->name('stages')->controller(stageController::class)->group(function () {
        //ステージマスタ情報を表示
        Route::get('stage', 'index')->name('stage.index');
        Route::get('logs', 'show_stage_log')->name('stages.log');
    });

    //島のルートをグループ化
    Route::prefix('lands')->name('lands')->controller(LandController::class)->group(function () {
        //島の情報を表示
        Route::get('land', 'index')->name('land');
        Route::get('landstatus', 'show_land_status')->name('show_land_status');

    });

    //ブロックのルートをグループ化
    Route::prefix('blocks')->name('blocks')->controller(blockController::class)->group(function () {
        //ブロックの情報を表示
        Route::get('blocks', 'index')->name('blocks');
    });


//プレイヤー一覧を表示する
    Route::get('users/userList', [UserListController::class, 'UserList'])->name('accounts.userList');

//アイテム一覧を表示する
    Route::get('accounts/itemList', [ItemListController::class, 'ItemList'])->name('accounts.itemList');

//所持アイテム一覧を表示する
    Route::get('accounts/useritemList', [UserItemListController::class, 'Useritem'])->name('accounts.useritemList');

//ログアウトする
    Route::post('accounts/dologout', [LoginController::class, 'dologout'])->name('accounts.dologout');
});

